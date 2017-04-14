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
using Newtonsoft.Json;
using SQLite;

namespace Parq.BusinessLayer.Contracts
{
    /// <summary>
    /// Business entity base class. Provides the ID property.
    /// </summary>
    public abstract class BusinessEntityBase : Interfaces.IBusinessEntity
    {


        /// <summary>
        /// Gets or sets the Database ID.
        /// </summary>
        [JsonProperty(PropertyName = "id"),PrimaryKey]
        public string Id { get; set; }

        [JsonProperty("createdAt")]
        public DateTime createdAtTime { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime updatedAtTime { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string version { get; set; }

    }
}
