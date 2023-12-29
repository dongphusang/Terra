import firebase_admin
from firebase_admin import firestore
from firebase_admin import credentials
from google.cloud.storage import Client, transfer_manager
import datetime

class FirestoreHelper():

    def __init__(self) -> None:
         # gcp creds  
        self._cred = credentials.Certificate('terra_gcp.json')
        self._app = firebase_admin.initialize_app(self._cred)
        self._db = firestore.client()

    def get_doc_content(self) -> str:
         # read data #
        _users_ref = self._db.collection("Subscriptions").document("NextWateringSchedule")
        self.schedule = _users_ref.get().to_dict().get("ESP32_1")

        return self.schedule

class StorageBucketHelper():

    def __init__(self) -> None:
        #gcp creds
        self._storage_client = Client.from_service_account_json('terra_gcp.json')
        self._bucket = self._storage_client.bucket('terra_html-email_images')
        self._filenames = ['graph_light.png', 'graph_tempXhumid.png']

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