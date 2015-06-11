using UnityEngine;
using System.Collections;
using System.Xml; 
using System.Xml.Serialization; 
using System.IO; 
using System.Text;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MousePlacement : MonoBehaviour {
	
	public Transform target;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	
	public float distanceMin = .5f;
	public float distanceMax = 15f;
	
	private Rigidbody rigidbody;
	
	float x = 0.0f;
	float y = 0.0f;
	
	string _FileLocation,_FileName; 

	UserData myData;
	string _data; 

	// Use this for initialization
	void Start () 
	{

		// Where we want to save and load to and from 
		_FileLocation=Application.dataPath; 
		_FileName="SaveData.xml"; 
		
		// we need soemthing to store the information into 
		myData=new UserData(); 
		
		
		LoadXML(); 
		if(_data.ToString() != "") 
		{ 
			// notice how I use a reference to type (UserData) here, you need this 
			// so that the returned object is converted into the correct type 
			myData = (UserData)DeserializeObject(_data); 
			// set the players position to the data we loaded 
			//VPosition=new Vector3(myData._iUser.x,myData._iUser.y,myData._iUser.z);              
			transform.position=myData._iUser.pos; 
			Debug.Log(myData._iUser.pos);
			transform.rotation=myData._iUser.rotation;
			distance = myData._iUser.distance;
			Debug.Log(myData._iUser.rotation);
			// just a way to show that we loaded in ok 
			Debug.Log("loaded trasnformation"); 
		}

		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		
		rigidbody = GetComponent<Rigidbody>();
		
		// Make the rigid body not change rotation
		if (rigidbody != null)
		{
			rigidbody.freezeRotation = true;
		}




	}
	
	void LateUpdate () 
	{
		if (target) 
		{
			if(Input.GetKey(KeyCode.LeftArrow))
			{
				x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
			}
			if(Input.GetKey(KeyCode.RightArrow))
			{
				y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			}
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			
			Quaternion rotation = Quaternion.Euler(y, x, 0);
			
			if(Input.GetKey(KeyCode.UpArrow))
			{
				distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel"), distanceMin, distanceMax);
			}
			RaycastHit hit;
			if (Physics.Linecast (target.position, transform.position, out hit)) 
			{
				distance -=  hit.distance;
			}
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;
			
			transform.rotation = rotation;
			transform.position = position;

			myData._iUser.rotation = transform.rotation; 
			myData._iUser.pos= transform.position; 
			myData._iUser.distance = distance;
			
			// Time to creat our XML! 
			_data = SerializeObject(myData); 
			// This is the final resulting XML from the serialization process 
			CreateXML(); 
		}
	}
	
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}

	/* The following metods came from the referenced URL */ 
	string UTF8ByteArrayToString(byte[] characters) 
	{      
		UTF8Encoding encoding = new UTF8Encoding(); 
		string constructedString = encoding.GetString(characters); 
		return (constructedString); 
	} 
	
	byte[] StringToUTF8ByteArray(string pXmlString) 
	{ 
		UTF8Encoding encoding = new UTF8Encoding(); 
		byte[] byteArray = encoding.GetBytes(pXmlString); 
		return byteArray; 
	} 
	
	// Here we serialize our UserData object of myData 
	string SerializeObject(object pObject) 
	{ 
		string XmlizedString = null; 
		MemoryStream memoryStream = new MemoryStream(); 
		XmlSerializer xs = new XmlSerializer(typeof(UserData)); 
		XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
		xs.Serialize(xmlTextWriter, pObject); 
		memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
		XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray()); 
		return XmlizedString; 
	} 
	
	// Here we deserialize it back into its original form 
	object DeserializeObject(string pXmlizedString) 
	{ 
		XmlSerializer xs = new XmlSerializer(typeof(UserData)); 
		MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString)); 
		XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8); 
		return xs.Deserialize(memoryStream); 
	} 
	
	// Finally our save and load methods for the file itself 
	void CreateXML() 
	{ 
		StreamWriter writer; 
		FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName); 
		if(!t.Exists) 
		{ 
			writer = t.CreateText(); 
		} 
		else 
		{ 
			t.Delete(); 
			writer = t.CreateText(); 
		} 
		writer.Write(_data); 
		writer.Close(); 
		Debug.Log("File written."); 
	} 
	
	void LoadXML() 
	{ 
		StreamReader r = File.OpenText(_FileLocation+"\\"+ _FileName); 
		string _info = r.ReadToEnd(); 
		r.Close(); 
		_data=_info; 
		Debug.Log("File Read"); 
	} 
} 

// UserData is our custom class that holds our defined objects we want to store in XML format 
public class UserData 
{ 
	// We have to define a default instance of the structure 
	public DemoData _iUser; 
	// Default constructor doesn't really do anything at the moment 
	public UserData() { } 
	
	// Anything we want to store in the XML file, we define it here 
	public struct DemoData 
	{ 
		public Quaternion rotation;
		public Vector3 pos;
		public float distance;
		
	} 

}
