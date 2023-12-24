import firebase_admin
from firebase_admin import firestore
from firebase_admin import credentials

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