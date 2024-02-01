import firebase_admin
from firebase_admin import firestore
from firebase_admin import credentials
from google.cloud.storage import Client, transfer_manager
import datetime
import json

class FirestoreHelper():

    def __init__(self) -> None:
         # gcp creds  
        self._cred = credentials.Certificate('terra_gcp.json')
        self._app = firebase_admin.initialize_app(self._cred)
        self._db = firestore.client()

    def get_doc_content(self) -> str:
         # read data #
        _users_ref = self._db.collection("Subscriptions").document("ESP32_1")
        self.schedule = _users_ref.get().to_dict().get("NextWateringSchedule")

        return self.schedule
    
    def get_water_dispensed(self) -> str:
        _users_ref = self._db.collection("Subscriptions").document("ESP32_1")
        self.water_dispensed = _users_ref.get().to_dict().get("WaterDispensed")
        
        return self.water_dispensed
    
    def download_emails(self):
        _users_ref = self._db.collection("Subscriptions").document("Active")
        self.water_dispensed = _users_ref.get().to_dict()
        
        with open("userList.json", "w") as outfile: 
            json.dump(self.water_dispensed, outfile)

class StorageBucketHelper():

    def __init__(self) -> None:
        #gcp creds
        self._storage_client = Client.from_service_account_json('terra_gcp.json')
        self._bucket = self._storage_client.bucket('terra_html-email_images')
        self._filenames = ['/home/graph_light.png', '/home/graph_tempXhumid.png']

    def upload_graphs_to_bucket(self) -> None:
        _results = transfer_manager.upload_many_from_filenames(
            self._bucket,
            self._filenames,
            source_directory='',
            worker_type=transfer_manager.THREAD,
            max_workers=8
        )
    
    def get_light_graph_url(self) -> str:
        _bucket = self._storage_client.get_bucket('terra_html-email_images')
        _expiration_time = datetime.datetime.utcnow() + datetime.timedelta(days=1)
        _signed_url = _bucket.blob('graph_light.png').generate_signed_url(
            expiration=_expiration_time,
            method='GET'
        )

        return _signed_url

    def get_temp_humid_url(self) -> str:
        _bucket = self._storage_client.get_bucket('terra_html-email_images')
        _expiration_time = datetime.datetime.utcnow() + datetime.timedelta(days=1)
        _signed_url = _bucket.blob('graph_tempXhumid.png').generate_signed_url(
            expiration=_expiration_time,
            method='GET'
        )

        return _signed_url