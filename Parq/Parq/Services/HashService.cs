#region Copyright
/*Copyright (c) 2016 Javus Software (Pty) Ltd

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
#endregion
using System;
using System.IO;
using Parq.Helpers;

namespace Parq.Services
{
	public class HashService
	{
		public static string CalculateMD5Hash (string s)
		{
			var md5 = MD5.Create ();
			var stream = GenerateStreamFromString (s);

			return BitConverter.ToString (md5.ComputeHash (stream)).Replace ("-", "").ToLower ();
		}

		private static Stream GenerateStreamFromString(string s)
		{
			var stream = new MemoryStream ();
			var writer = new StreamWriter (stream);

			writer.Write(s);
			writer.Flush();
			stream.Position = 0;

			return stream;
		}
	}
}