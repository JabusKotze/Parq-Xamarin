## Ranging Example Code

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using AltBeaconOrg.BoundBeacon;
	using Android.App;
	using Android.Content;
	
	using Android.Graphics;
	using Android.OS;
	using Android.Util;
	using Android.Widget;
	
	namespace AndroidAltBeaconLibrary.Sample
	{
		[Activity(Label = "AltBeacon Sample", 
			MainLauncher = true,
			LaunchMode = Android.Content.PM.LaunchMode.SingleInstance,
			Theme = "@style/Theme.AltBeacon",
			Icon = "@drawable/altbeacon")]
		public class MainActivity : Activity, IDialogInterfaceOnDismissListener, IBeaconConsumer
		{
			private readonly RangeNotifier _rangeNotifier;
	
			AltBeaconOrg.BoundBeacon.Region _tagRegion, _emptyRegion;
	
			private Button _backgroundButton, _stopButton, _startButton;
			private ListView _list;
			private BeaconManager _beaconManager;
			private readonly List<Beacon> _data;
	
			public MainActivity()
			{
				_rangeNotifier = new RangeNotifier();
				_data = new List<Beacon>();
			}
	
			protected override void OnCreate(Bundle bundle)
			{
				base.OnCreate(bundle);
	
				SetContentView(Resource.Layout.MainActivity);
	
				_backgroundButton = FindViewById<Button>(Resource.Id.backgroundButton);
				_stopButton = FindViewById<Button>(Resource.Id.stopButton);
				_startButton = FindViewById<Button>(Resource.Id.startButton);
				_list = FindViewById<ListView>(Resource.Id.list);
	
				_list.Adapter = new ListSource((data, position, convertView, parent) => 
				{
					var view = convertView;
					var beacon = data[position];
	
					if (view == null)
					{
						view  = LayoutInflater.Inflate(Resource.Layout.ListItem, parent, false);
					}
		
					view.FindViewById<TextView>(Resource.Id.beaconId).Text = beacon.Id1.ToString().ToUpper();
					view.FindViewById<TextView>(Resource.Id.beaconDistance).Text = string.Format("{0:N2}m", beacon.Distance);
	
					return view;
				});
	
				VerityBluetooth();
	
				_beaconManager = BeaconManager.GetInstanceForApplication(this);

				var iBeaconParser = new BeaconParser();
				//	Estimote > 2013
				iBeaconParser.SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24");
				_beaconManager.BeaconParsers.Add(iBeaconParser);
				_beaconManager.Bind(this);	
				_rangeNotifier.DidRangeBeaconsInRegionComplete += RangingBeaconsInRegion;
			}
	
			protected override void OnResume()
			{
				base.OnResume();
	
				((BeaconReferenceApplication) this.ApplicationContext).MainActivity = this;
	
				_backgroundButton.Click += OnBackgroundClick;
				_stopButton.Click += OnStopClick;
				_startButton.Click += OnStartClick;
	
				if(_beaconManager.IsBound(this))
				{
					_beaconManager.SetBackgroundMode(false);
				}
			}
	
			void OnStopClick (object sender, EventArgs e)
			{
				_stopButton.Enabled = false;
				_startButton.Enabled = true;
	
				_beaconManager.StopRangingBeaconsInRegion(_tagRegion);
				_beaconManager.StopRangingBeaconsInRegion(_emptyRegion);
			}
	
			async void OnStartClick(object sender, EventArgs e)
			{
				_startButton.Enabled = false;
				_stopButton.Enabled = true;
	
				await ClearData();
	
				_beaconManager.StartRangingBeaconsInRegion(_tagRegion);
				_beaconManager.StartRangingBeaconsInRegion(_emptyRegion);
			}
	
			protected override void OnPause()
			{
				base.OnPause();
	
				_backgroundButton.Click -= OnBackgroundClick;
				_stopButton.Click -= OnStopClick;
				_startButton.Click -= OnStartClick;
	
				if(_beaconManager.IsBound(this))
				{
					_beaconManager.SetBackgroundMode(true);
				}
	
				((BeaconReferenceApplication) this.ApplicationContext).MainActivity = null;
			}
	
			protected override void OnDestroy()
			{
				base.OnDestroy();
				if(_beaconManager.IsBound(this)) _beaconManager.Unbind(this);
			}
	
			async void RangingBeaconsInRegion(object sender, RangeEventArgs e)
			{
				await ClearData();
	
				var allBeacons = new List<Beacon>();
				if(e.Beacons.Count > 0)
				{
					foreach(var b in e.Beacons)
					{
						allBeacons.Add(b);
					}
	
					var orderedBeacons = allBeacons.OrderBy(b => b.Distance).ToList();
					await UpdateData(orderedBeacons);
				}
				else
				{
					// unknown
					await ClearData();
				}
			}
	
			void RemoveBeaconsNoLongerVisible(List<Beacon> allBeacons)
			{
				if(allBeacons == null || allBeacons.Count == 0) return;
	
				var delete = new List<Beacon>();
				foreach(var d in _data)
				{
					if(allBeacons.All(ab => ab.Id1.ToString() != d.Id1.ToString()))
					{
						delete.Add(d);
					}
				}
		
				_data.RemoveAll(d => delete.Any(del => del.Id1.ToString() == d.Id1.ToString()));
	
				if(delete.Count > 0)
				{
					delete = null;
					UpdateList();
				}
			}
	
			void OnBackgroundClick(object sender, EventArgs e)
			{
				var intent = new Intent(this, typeof(BackgroundActivity));
				StartActivity(intent);
			}
	
			void VerityBluetooth()
			{
				try 
				{
					if (!BeaconManager.GetInstanceForApplication(this).CheckAvailability())
					{
						var builder = new AlertDialog.Builder(this);
						builder.SetTitle("Bluetooth not enabled");
						builder.SetMessage("Please enable bluetooth in settings and restart this application.");
						EventHandler<DialogClickEventArgs> handler = null;
						builder.SetPositiveButton(Android.Resource.String.Ok, handler);
						builder.SetOnDismissListener(this);
						builder.Show();
					}		
				}
				catch (BleNotAvailableException e) 
				{
					Log.Debug("BleNotAvailableException", e.Message);
	
					var builder = new AlertDialog.Builder(this);
					builder.SetTitle("Bluetooth LE not available");
					builder.SetMessage("Sorry, this device does not support Bluetooth LE.");
					EventHandler<DialogClickEventArgs> handler = null;
					builder.SetPositiveButton(Android.Resource.String.Ok, handler);
					builder.SetOnDismissListener(this);
					builder.Show();
				}
			}
	
			private async Task UpdateDisplay(string message, Color color = default(Color))
			{
			}
	
			private async Task UpdateData(List<Beacon> beacons)
			{
				await Task.Run(() =>
				{	
					var newBeacons = new List<Beacon>();
					foreach(var beacon in beacons)
					{
						if(_data.All(b => b.Id1.ToString() == beacon.Id1.ToString()))
						{
							newBeacons.Add(beacon);
						}
					}
	
					RunOnUiThread(() =>
					{
						foreach(var beacon in newBeacons)
						{
							_data.Add(beacon);
						}
	
						if (newBeacons.Count > 0)
						{
							_data.Sort((x,y) => x.Distance.CompareTo(y.Distance));
							UpdateList();
						}
					});
				});
			}
	
			async void EnteredRegion(object sender, MonitorEventArgs e)
			{
				await UpdateDisplay("A new beacon just showed up!");
			}
	
			async void ExitedRegion(object sender, MonitorEventArgs e)
			{
				await UpdateDisplay("They went away :(");
			}
	
			async void DeterminedStateForRegionComplete(object sender, MonitorEventArgs e)
			{
				await UpdateDisplay("I have just switched from seeing/not seeing beacons: " + e.State);
			}
	
			public bool BindService(Intent p0, IServiceConnection p1, int p2)
			{
				return true;
			}
	
			public void OnBeaconServiceConnect()
			{
				_beaconManager.SetForegroundBetweenScanPeriod(5000); // 5000 milliseconds
	
				_beaconManager.SetRangeNotifier(_rangeNotifier);
	
				_tagRegion = new AltBeaconOrg.BoundBeacon.Region("myUniqueBeaconId", Identifier.Parse("2F234454-CF6D-4A0F-ADF2-F4911BA9FFA6"), null, null);
				_emptyRegion = new AltBeaconOrg.BoundBeacon.Region("myEmptyBeaconId", null, null, null);
	
				_beaconManager.StartRangingBeaconsInRegion(_tagRegion);
				_beaconManager.StartRangingBeaconsInRegion(_emptyRegion);
	
				_startButton.Enabled = false;
			}
	
			public void OnDismiss(IDialogInterface dialog)
			{
				Finish();
			}
	
			private async Task ClearData()
			{
				RunOnUiThread(() =>
				{
					_data.Clear();
					((ListSource)_list.Adapter).UpdateList(_data);
				});
			}
	
			private void UpdateList()
			{
				RunOnUiThread(() => 
				{
					((ListSource)_list.Adapter).UpdateList(_data);
				});
			}
		}
	
		public class ListSource : BaseAdapter<Beacon>
		{
			private List<Beacon> _data;
			private Func<List<Beacon>, int, Android.Views.View, Android.Views.ViewGroup, Android.Views.View> _getView;
	
			public ListSource(Func<List<Beacon>, int, Android.Views.View, Android.Views.ViewGroup, Android.Views.View> getView)
			{
				_getView = getView;
				_data = new List<Beacon>();
			}
	
			public override long GetItemId(int position)
			{
				return position;
			}
			public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
			{
				return _getView(_data, position, convertView, parent);
			}
	
			public override int Count
			{
				get
				{
					return _data.Count;
				}
			}
	
			public override Beacon this[int index]
			{
				get
				{
					return _data[index];
				}
			}
	
			public void UpdateList(List<Beacon> list)
			{
				_data = list;
				NotifyDataSetChanged();				
			}
		}
	}