using CommunityToolkit.Maui.Core.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terra.TerraConstants;

namespace Terra.Services
{
   
    public class FirestoreService
    {
        private GoogleCredential credential;
        private FirestoreDb firestore;
        private DocumentReference docRef;

        public FirestoreService()
        {
            credential = GoogleCredential.FromJson(GetAccountSetting());
            firestore = FirestoreDb.Create("terra-44a87", new FirestoreClientBuilder
            {
                Credential = credential
            }.Build());
        }

        /// <summary>
        /// Get content from accountSetting.json
        /// </summary>
        /// <returns> configuration builder </returns>
        private string GetAccountSetting()
        {
            Assembly assembly = Assembly.GetExecutingAssembly(); // get current assembly
            using Stream stream = assembly.GetManifestResourceStream("Terra.accountSetting.json");
            using StreamReader reader = new(stream);
            var json = reader.ReadToEnd();
            return json;
        }

        /*/// <summary>
        /// DORMANT METHOD
        /// </summary>
        /// <param name="collection"> Firestore collection. </param>
        /// <param name="document"> Firestore document. </param>
        /// <returns> Observable collection of dictionary keys. </returns>
        public async Task<ObservableCollection<string>> GetKeysAsCollection(string collection, string document)
        {
            docRef = firestore.Collection(collection).Document(document);
            DocumentSnapshot dictionary = await docRef.GetSnapshotAsync();

            return new ObservableCollection<string>(dictionary.ToDictionary().Keys.ToObservableCollection());
        }*/

        /// <summary>
        ///  
        /// </summary>
        /// <param name="key"></param>
        /// <param name="collection"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<List<object>> GetValue(string key, string collection, string document)
        {
            docRef = firestore.Collection(collection).Document(document);
            var dictionary = (await docRef.GetSnapshotAsync()).ToDictionary();
            
            if (dictionary.ContainsKey(key))
            {
                return new List<object>((List<object>)dictionary[key]);
            }
            else return new List<object>();
        }

        /// <summary>
        /// Upload key-value data to firestore.
        /// </summary>
        /// <param name="key"> Dictionary key. </param>
        /// <param name="value"> Dictionary value. </param>
        /// <param name="collection"> Target firestore collection. </param>
        /// <param name="document"> Target firestore document of said collection. </param>
        /// <returns> Firestore write-result. </returns>
        public Task Post(string key, object value, string collection, string document)
        {
            docRef = firestore.Collection(collection).Document(document);
            Dictionary<string, object> data = new()
            {
                {key, value},
            };

            return docRef.SetAsync(data, SetOptions.MergeAll);
        } 

        /// <summary>
        /// Remove a key from firestore document.
        /// </summary>
        /// <param name="key"> Dictionary key. </param>
        /// <param name="collection"> Target firestore collection. </param>
        /// <param name="document"> Target firestore document of said collection. </param>
        /// <returns> Firestore write-result. </returns>
        public Task Remove(string key, string collection, string document)
        {
            docRef = firestore.Collection(collection).Document(document);
            Dictionary<string, object> data = new()
            {
                {key, FieldValue.Delete },
            };
            try
            {
                return docRef.UpdateAsync(data);
            }
            catch (NullReferenceException)
            {
                return Task.CompletedTask;
            }          
        }

        /// <summary>
        /// Remove an element from all documents within a parent collection.
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="formatted_mail"></param>
        /// <returns></returns>
        public async Task RemoveFromParentCollection(string mail, string formatted_mail)
        {
            // removal from Active document
            var collection = FirestoreConstant.SUBSCRIPTION;
            var document = FirestoreConstant.ACTIVE_EMAILS;

            docRef = firestore.Collection(collection).Document(document);
            var dictionary = (await docRef.GetSnapshotAsync()).ToDictionary();

            // iterate through all collection within the document to find target element
            foreach (KeyValuePair<string, object> kvp in dictionary)
            {
                var array = (List<object>)kvp.Value;
                if (array.Contains(mail))
                {                   
                    // if child collection has one element, remove the key anyway
                    if (array.Count == 1)
                    {                       
                        await Remove(kvp.Key, collection, document);
                        return;
                    }
                    // else, remove only an element of the key
                    array.Remove(mail);
                    var new_dict = new Dictionary<string, object>()
                    {
                        [kvp.Key] = array
                    };

                    // upload new key-value data to firestore and replace old one
                    await docRef.SetAsync(new_dict, SetOptions.MergeAll);                   
                }
            }

            // removal from Inactive document
            document = FirestoreConstant.INACTIVE_EMAILS;
            await Remove(formatted_mail, collection, document);
        }
    }
}
