using UnityEngine; // 41 Post - Created by DimasTheDriver on Apr/20/2012 . Part of the 'Unity: Animated texture from image sequence' post series. Available at: http://www.41post.com/?p=4742 
using System.Collections; //Script featured at Part 2 of the post series.

public class ImageSequenceSingleTexture : MonoBehaviour 
{
	//A texture object that will output the animation
	private Texture texture;
	//With this Material object, a reference to the game object Material can be stored
	private Material goMaterial;
	//An integer to advance frames
	private int frameCounter = 0;
	
	//A string that holds the name of the folder which contains the image sequence
	public string folderName;
	//The name of the image sequence
	public string imageSequenceName;
	//The number of frames the animation has
	public int numberOfFrames;
	
	//The base name of the files of the sequence
	private string baseName;
	
	void Awake()
	{
		//Get a reference to the Material of the game object this script is attached to
		this.goMaterial = this.GetComponent<Renderer>().material;
		//With the folder name and the sequence name, get the full path of the images (without the numbers)
		this.baseName = this.folderName + "/" + this.imageSequenceName;
	}
	
	void Start () 
	{
		//set the initial frame as the first texture. Load it from the first image on the folder
		texture = (Texture)Resources.Load(baseName + "00000", typeof(Texture));
	}
	
	void Update () 
	{
		//Start the 'PlayLoop' method as a coroutine with a 0.04 delay  
		//StartCoroutine("PlayLoop", 0.04f);
		if (Input.GetKey ("a")) 
		{
			//advance one frame
			frameCounter = (++frameCounter)%numberOfFrames;
			
			//load the current frame
			this.texture = (Texture)Resources.Load(baseName + frameCounter.ToString("D5"), typeof(Texture));

		}

		if (Input.GetKey ("s")) 
		{
			//advance one frame
			frameCounter = (--frameCounter)%numberOfFrames;
			
			//load the current frame
			this.texture = (Texture)Resources.Load(baseName + frameCounter.ToString("D5"), typeof(Texture));
			
		}
		//Set the material's texture to the current value of the frameCounter variable
		goMaterial.mainTexture = this.texture;
	}
	
	//The following methods return a IEnumerator so they can be yielded:
	//A method to play the animation in a loop
    IEnumerator PlayLoop(float delay)  
    {  
        //wait for the time defined at the delay parameter  
        yield return new WaitForSeconds(delay);  
        
		//advance one frame
		frameCounter = (++frameCounter)%numberOfFrames;
		
		//load the current frame
		this.texture = (Texture)Resources.Load(baseName + frameCounter.ToString("D5"), typeof(Texture));
		
        //Stop this coroutine  
        StopCoroutine("PlayLoop");  
    }
	
	//A method to play the animation just once
    IEnumerator Play(float delay)  
    {  
        //wait for the time defined at the delay parameter  
        yield return new WaitForSeconds(delay);  
        
		//if it isn't the last frame
		if(frameCounter < numberOfFrames-1)
		{
			//Advance one frame
			++frameCounter;
			
			//load the current frame
			this.texture = (Texture)Resources.Load(baseName + frameCounter.ToString("D5"), typeof(Texture));
		}

        //Stop this coroutine  
        StopCoroutine("Play");  
    }
}
