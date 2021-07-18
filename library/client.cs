﻿using System;
using PluralkitAPI.Models;
using RestSharp;

namespace PluralkitAPI
{
    namespace Client {
        /// <summary>The Client class that handles all requests.</summary>
        /// <param name="token">A system's pluralkit authorization token. Optional.</param>
        public class PKClient
        {
            public string? token;
            RestClient client = new RestClient("https://api.pluralkit.me/v1/");

            private IRestRequest AddHeaders(IRestRequest request, bool ContentHeader) { 
                request.AddHeader("Authorization", token);
                if (ContentHeader) request.AddHeader("Content-Type", "application/json");
                return request;
            }
            
            /// <summary> Fetches the specified system. </summary>
            /// <param name="system">The system to fetch, can be a discord snowflake id, a pluralkit system id, or null to fetch the system associated with the authorization token passed..</param>
            /// <returns>The fetched <see cref="System"/> object.</returns>
            public Models.System GetSystem(string? system)
            {   
                IRestRequest request;
                if (system == null && token != null) { 
                    request = new RestRequest("s/");
                } else if (system.Length == 5 && system != null) {
                    request = new RestRequest("s/{system}")
                        .AddUrlSegment("system", system);
                } else if ((system.Length ==  17 || system.Length == 18 || system.Length == 19) && system != null) { 
                    request = new RestRequest("a/{system}")
                        .AddUrlSegment("system", system);
                } else {
                    throw new Exception("Must be a system ID, discord user ID, or null to use the system associated with the token you may have passed.");
                }
                
                request = AddHeaders(request, false);

                var response = client.Get(request);
                switch (((int)response.StatusCode)) {
                    case 404: 
                        throw new Exception("System not found.");
                    case 403:
                        throw new Exception("Access denied.");
                    case 401: 
                        throw new Exception("Unauthorized.");
                }
                return Models.System.FromJson(response.Content);
            } 
            /// <summary>Edits the system associated with the token passed.</summary>
            /// <param name="system">The <see cref="System"/> object to change your system to match.</param>
            /// <returns>The updated <see cref="System"/> object.</returns>
            public Models.System EditSystem(Models.System system) {
                var request = new RestRequest("s/");
                
                var response = client.Patch(request);

                switch (((int)response.StatusCode)) { 
                    case 403:
                        throw new Exception("Access denied.");
                    case 401: 
                        throw new Exception("Unauthorized.");
                }
                return Models.System.FromJson(response.Content);
            }
            /// <summary>Gets the specified member.</summary>
            /// <param name="member"> The member to get, must be a pluralkit member id." </param>
            /// <returns>The fetched <see cref="Member"/> object.</returns>
            public Models.Member GetMember(string member)
            {   
                IRestRequest request = new RestRequest("m/{member}")
                    .AddUrlSegment("member", member);
                var response = client.Get(request);
                switch (((int)response.StatusCode)) {
                    case 404: 
                        throw new Exception("Member not found.");
                    case 403:
                        throw new Exception("Access denied.");
                    case 401: 
                        throw new Exception("Unauthorized.");
                }
                return Models.Member.FromJson(response.Content);
            }
        /*
        public string GetSwitches(string system)
        {
            var request = new RestRequest("s/switches");

        } */
        }
    }
}