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

        /// <summary>
        /// Get dictionary keys from a document, and return them as a observable collection.
        /// [Usage]: Get a list of active emails.
        /// </summary>
        /// <param name="collection"> Firestore collection. </param>
        /// <param name="document"> Firestore document. </param>
        /// <returns> Observable collection of dictionary keys. </returns>
        public async Task<ObservableCollection<string>> GetKeysAsCollection(string collection, string document)
        {
            docRef = firestore.Collection(collection).Document(document);
            DocumentSnapshot dictionary = await docRef.GetSnapshotAsync();

            return new ObservableCollection<string>(dictionary.ToDictionary().Keys.ToObservableCollection());
        }

        /// <summary>
        /// Upload key-value data to firestore.
        /// </summary>
        /// <param name="key"> Dictionary key. </param>
        /// <param name="value"> Dictionary value. </param>
        /// <param name="collection"> Target firestore collection. </param>
        /// <param name="document"> Target firestore document of said collection. </param>
        /// <returns> Firestore write-result. </returns>
        public Task Post(string key, string value, string collection, string document)
        {
            docRef = firestore.Collection(collection).Document(document);
            Dictionary<string, string> data = new()
            {
                {key, value},
            };

            return docRef.SetAsync(data, SetOptions.MergeAll);
        } 

        /// <summary>
        /// Remove key-value data from firestore.
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
    }
}
