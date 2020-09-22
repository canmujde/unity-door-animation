using UnityEngine;
using System.Collections;

public class Pure_FPP_Controller : MonoBehaviour {

	//Landing sound tylko jak controller dłużej spadał :<
	//jest przerwa po skoku w walk soundzie

	[Space(10)]
	[Tooltip("The Character Controller to operate EFPS. IT's required to work.")]
	private CharacterController Controller; 
	[Tooltip("FPS Camera's transform. Used at crouching height change.")]
	public Camera CharacterCamera; // it's position is used. It shouldn't be moved by animations or other type of external transform movement.
	[Tooltip("If you wish to use any sounds, place the desired AudioSource here.")]
	public AudioSource SoundSource;
	[Tooltip("Gravity force power.")]
	public float Gravity = 1f;
	[Space(10)]
	[Tooltip("The primary speed of this character.")]
	public float MoveSpeed = 6;
	[Tooltip("If you wish to use Walking footstep sounds, specify how many Walk Sounds there are to randomize from.")]
	[Range(0,5)]
	public int MaxWalkSounds;
	public AudioClip WalkSound1;
	public AudioClip WalkSound2;
	public AudioClip WalkSound3;
	public AudioClip WalkSound4;
	public AudioClip WalkSound5;
	[Tooltip("The Input Axis of moving front and back")]
	public string ForwardInput = "Vertical";
	[Tooltip("The Input Axis of moving sideways.")]
	public string SidewaysInput = "Horizontal";

	[Space(10)]
	[Tooltip("The Input Button for crouching")]
	public string CrouchInput = "Crouch";
	[Tooltip("The move speed while crouching")]
	public float CrouchSpeed = 3;
	[Tooltip("If you wish to use Crouch footstep sounds, specify how many Crouch Sounds there are to randomize from.")]
	[Range(0,5)]
	public int MaxCrouchSounds;
	public AudioClip CrouchSound1;
	public AudioClip CrouchSound2;
	public AudioClip CrouchSound3;
	public AudioClip CrouchSound4;
	public AudioClip CrouchSound5;

	[Space(10)]
	[Tooltip("The Input Button for Sprint")]
	public string RunInput = "Sprint";
	[Tooltip("The speed of the run.")]
	public float RunSpeed = 9; 
	[Tooltip("If you wish to use Running footstep sounds, specify how many Run Sounds there are to randomize from.")]
	[Range(0,5)]
	public int MaxRunSounds;
	public AudioClip RunSound1;
	public AudioClip RunSound2;
	public AudioClip RunSound3;
	public AudioClip RunSound4;
	public AudioClip RunSound5;


	[Space(10)]
	[Tooltip("How powerful the jump should be.")]
	public float JumpSpeed = 10;
	[Tooltip("InputButton for Jumping")]
	public string JumpInput = "Jump";
	[Tooltip("Time cooldown between jumps.")]
	public float JumpReuse = 0.1f;
	[Tooltip("Specify layers of objects which the player may hit with his head while jumping (like ceilings in rooms). If he jumps and straight above him is said object, he will loose his momentum.")]
	public LayerMask HeadHitCheckLayers; //this checks if during a jump the player has hit something with his head.
	[Range(0,5)]
	[Tooltip("If you wish to use Jump sounds, specify how many Jump Sounds there are to randomize from.")]
	public int MaxJumpSounds;
	public AudioClip JumpSound1;
	public AudioClip JumpSound2;
	public AudioClip JumpSound3;
	public AudioClip JumpSound4;
	public AudioClip JumpSound5;
	[Space(2)]
	[Tooltip("Jump landing sound - if you want to use one")]
	public AudioClip JumpLandSound;
	[Tooltip("After how many second of falling should the Landing Sound be played upon grounding?")]
	public float LandSoundFallTime = 0.3f;
	[Tooltip("You can use Multijump to jump more than once in mid air.")]
	public int MultiJump = 2;
	[Tooltip("You can specify an object which will recieve a SendMessage function called Multijump. Useful for camera postprocess or other effects.")]
	public GameObject MultiJumpMessageReciever;
	[Tooltip("If you wish to use Multijump sounds, specify how many of them there are to randomize from.")]
	[Range(0,5)]
	public int MaxMultiJumpSounds;
	public AudioClip MultiJumpSound1;
	public AudioClip MultiJumpSound2;
	public AudioClip MultiJumpSound3;
	public AudioClip MultiJumpSound4;
	public AudioClip MultiJumpSound5;








	private float  InputVert; //Input value (-1.0 - 1.0)
	private float  InputHor;	//same as above
	private int JumpsAvailable;
	private bool JumpBlock;		
	private bool SlopeJumpBlock;
	private float SlopeAngle;
	private float tempJumpSpeed;
	private float jt; //jump timer
	private RaycastHit SlopeCheck;
	private RaycastHit CrouchHeadCheck;
	private Vector3 CamPos; //camera starting position
	private Vector3 tempCamPos; //cam position
	private float ControllerStartingHeight; //staring height of Controller
	private Vector3 CameraVector; //a "dirty" value to store temporary camera position;
	private bool CrouchingBlocked;
	private int RollMultiJumpSound;
	private int RollJumpSound;
	private bool tempGrounded; //was grounded in the previous frame?
	private int RollWalkSound;
	private int RollRunSound;
	private int RollCrouchSound;
	private float ft; //falling timer
	


	private float TheX;	//Calculated movement vector variables applied to  Character Controller
	private float TheZ;
	private float TheY;
	private Vector3 TheVector; //Movement vector, applied to the Character Controller
	private bool HalfSpeed; // this is used to prevent diagonal acceleration bug (faster diagonal movement than normal)

	void Start()
	{
		if (GetComponent<CharacterController> () == null) {
			Debug.LogError ("I am terribly sorry, but it seems that object named " + gameObject.name + " does not have a Character Controller Component attached. It is required for the Essential FPS Controller to work!");
		} else {
			Controller = GetComponent<CharacterController> ();
		}
		if (CharacterCamera != null) {
			CamPos = CharacterCamera.transform.localPosition;
		}
		ControllerStartingHeight = Controller.height;
	}

	void Update()
	{


		if (Controller.isGrounded == true && (SoundSource.clip == JumpSound1 || SoundSource.clip == JumpSound2 || SoundSource.clip == JumpSound3 || SoundSource.clip == JumpSound4 || SoundSource.clip == JumpSound5)) 
		{
			SoundSource.Stop();
			SoundSource.clip = null;
		}


		if (tempGrounded == false && Controller.isGrounded == true && JumpLandSound != null && ft > LandSoundFallTime && SoundSource != null) {
			SoundSource.Stop ();
			SoundSource.clip = JumpLandSound;
			SoundSource.Play();

		}

		if (Controller.isGrounded && JumpBlock == false)   
		{  
			JumpsAvailable = MultiJump;
			tempJumpSpeed = 0;
		}


		InputVert = Input.GetAxis(ForwardInput);
		InputHor = Input.GetAxis (SidewaysInput);



		if ((InputVert > 0.1 || InputVert < -0.1) && (InputHor > 0.1 || InputHor < -0.1)) {
			HalfSpeed = true;
		} else {
			HalfSpeed = false;
		}
		

		if (!string.IsNullOrEmpty(JumpInput) && SlopeJumpBlock != true) 
		{
			if (Input.GetButtonDown (JumpInput) && JumpsAvailable > 0 && JumpBlock == false) 
			{
			
				JumpsAvailable -= 1;
				JumpBlock = true;
				tempJumpSpeed = JumpSpeed;

				if (JumpsAvailable == MultiJump - 1) //Normal Jump
				{
					if (SoundSource != null) 
					{
						if(MaxJumpSounds >0)
						{
							RollJumpSound = Random.Range(0,MaxJumpSounds);
							if(RollJumpSound == 0)
							{
								SoundSource.Stop ();
								SoundSource.clip = JumpSound1;
								SoundSource.Play ();
							}
							if(RollJumpSound == 1)
							{
								SoundSource.Stop ();
								SoundSource.clip = JumpSound2;
								SoundSource.Play ();
							}
							if(RollJumpSound == 2)
							{
								SoundSource.Stop ();
								SoundSource.clip = JumpSound3;
								SoundSource.Play ();
							}
							if(RollJumpSound == 3)
							{
								SoundSource.Stop ();
								SoundSource.clip = JumpSound4;
								SoundSource.Play ();
							}
							if(RollJumpSound == 4)
							{
								SoundSource.Stop ();
								SoundSource.clip = JumpSound5;
								SoundSource.Play ();
							}

						}
					}
				}
				if (JumpsAvailable < MultiJump - 1 ) //Multijump
				{
					if(MultiJumpMessageReciever != null)
					{
						MultiJumpMessageReciever.SendMessage("MultiJump", SendMessageOptions.DontRequireReceiver);
					}
						if(MaxMultiJumpSounds >0)
					{
						
						RollMultiJumpSound = Random.Range(0,MaxMultiJumpSounds);
							if(RollMultiJumpSound == 0)
							{
							SoundSource.PlayOneShot(MultiJumpSound1);
							}
							if(RollMultiJumpSound == 1)
							{
							SoundSource.PlayOneShot(MultiJumpSound2);
							}
							if(RollMultiJumpSound == 2)
							{
							SoundSource.PlayOneShot(MultiJumpSound3);
							}
							if(RollMultiJumpSound == 3)
							{
							SoundSource.PlayOneShot(MultiJumpSound4);
							}
							if(RollMultiJumpSound == 4)
							{
							SoundSource.PlayOneShot(MultiJumpSound5);
							}

						}
					}
				}
			}


		if (JumpBlock == true) 
		{
			jt += Time.deltaTime;
			if(jt > JumpReuse)
			{
				JumpBlock = false;
				jt = 0;
			}
		}


		if (Controller.isGrounded == true) {
			Debug.DrawRay(Controller.transform.position, -Controller.transform.up,Color.blue);
			if(Physics.Raycast(Controller.transform.position, -Controller.transform.up, out SlopeCheck))
			{
				SlopeAngle = Vector3.Angle (SlopeCheck.normal, Controller.transform.up);

				if(SlopeAngle < Controller.slopeLimit)
				{		
					SlopeJumpBlock = false;
				}
				if(SlopeAngle >= Controller.slopeLimit)
				{		
					SlopeJumpBlock = true;
				}
			}
		}

		if (Physics.Raycast (Controller.transform.position, Controller.transform.up, out CrouchHeadCheck)) {
			if (CrouchHeadCheck.distance <= ControllerStartingHeight * 0.52f) {
				CrouchingBlocked = true;
				Debug.DrawRay (Controller.transform.position, Controller.transform.up, Color.red);
			} 
			if (CrouchHeadCheck.distance > ControllerStartingHeight * 0.52f || CrouchHeadCheck.transform == null) {
				CrouchingBlocked = false;
				Debug.DrawRay (Controller.transform.position, Controller.transform.up, Color.white);
			}
		} else {
			CrouchingBlocked = false;Debug.DrawRay (Controller.transform.position, Controller.transform.up, Color.white);
		}

	
		if ((Input.GetButton (CrouchInput) || CrouchingBlocked) && Controller.isGrounded) {
			
			if (CharacterCamera != null) {
				CameraVector = new Vector3 (CharacterCamera.transform.localPosition.x, CamPos.y * 0.49f, CharacterCamera.transform.localPosition.z);
				CharacterCamera.transform.localPosition = CameraVector;
			}
			Controller.height = 0.49f * ControllerStartingHeight;
		}
		if (!Input.GetButton (CrouchInput) && CrouchingBlocked == false) {
			if (CharacterCamera != null) {
				CameraVector = new Vector3 (CharacterCamera.transform.localPosition.x, CamPos.y, CharacterCamera.transform.localPosition.z);
				CharacterCamera.transform.localPosition = CameraVector;
			}
			Controller.height = ControllerStartingHeight;
		}



		//WalkSounds
		if(SoundSource != null && SoundSource.isPlaying == false && (Controller.isGrounded || ft <= LandSoundFallTime) && (Input.GetButton(ForwardInput) || Input.GetButton(SidewaysInput)) && !Input.GetButton(RunInput) && !Input.GetButton(CrouchInput))
		   {
			if(MaxWalkSounds >0)
			{
				RollWalkSound = Random.Range(0, MaxWalkSounds);
				if(RollWalkSound == 0)
				{
					SoundSource.clip = WalkSound1;
					SoundSource.Play();
				}
				if(RollWalkSound == 1)
				{
					SoundSource.clip = WalkSound2;
					SoundSource.Play();
				}
				if(RollWalkSound == 2)
				{
					SoundSource.clip = WalkSound3;
					SoundSource.Play();
				}
				if(RollWalkSound == 3)
				{
					SoundSource.clip = WalkSound4;
					SoundSource.Play();
				}
				if(RollWalkSound == 4)
				{
					SoundSource.clip = WalkSound5;
					SoundSource.Play();
				}
			}
		}
		if (SoundSource != null && SoundSource.isPlaying == false && Controller.isGrounded && (Input.GetButton (ForwardInput) || Input.GetButton (SidewaysInput)) && Input.GetButton (RunInput) && !Input.GetButton (CrouchInput)) {

			if(MaxRunSounds >0){
			RollRunSound = Random.Range(0, MaxRunSounds);
			if(RollRunSound == 0)
			{
				SoundSource.clip = RunSound1;
				SoundSource.Play();
			}
			if(RollRunSound == 1)
			{
				SoundSource.clip = RunSound2;
				SoundSource.Play();
			}
			if(RollRunSound == 2)
			{
				SoundSource.clip = RunSound3;
				SoundSource.Play();
			}
			if(RollRunSound == 3)
			{
				SoundSource.clip = RunSound4;
				SoundSource.Play();
			}
			if(RollRunSound == 4)
			{
				SoundSource.clip = RunSound5;
				SoundSource.Play();
			}
			}
		}

		if (SoundSource != null && SoundSource.isPlaying == false && Controller.isGrounded && (Input.GetButton (ForwardInput) || Input.GetButton (SidewaysInput)) && Input.GetButton (CrouchInput)) {
			
		if(MaxCrouchSounds >0){
		RollCrouchSound = Random.Range(0, MaxCrouchSounds);
			if(RollCrouchSound == 0)
			{
				SoundSource.clip = CrouchSound1;
				SoundSource.Play();
			}
			if(RollCrouchSound == 1)
			{
				SoundSource.clip = CrouchSound2;
				SoundSource.Play();
			}
			if(RollCrouchSound == 2)
			{
				SoundSource.clip = CrouchSound3;
				SoundSource.Play();
			}
			if(RollCrouchSound == 3)
			{
				SoundSource.clip = CrouchSound4;
				SoundSource.Play();
			}
			if(RollCrouchSound == 4)
			{
				SoundSource.clip = CrouchSound5;
				SoundSource.Play();
			}
		}
		}
		//Debug ray for user's convenience - shows Controller's front
		Debug.DrawRay (Controller.transform.position, Controller.transform.forward, Color.white); 

		tempGrounded = Controller.isGrounded;


		if (Controller.isGrounded == false) {			//These two control the landing sound effect.
			ft += Time.deltaTime;
		}
		if (Controller.isGrounded == true) {
			ft = 0;
		}
	}


	void FixedUpdate()
	{
		TheVector = Vector3.zero;

		if (!Input.GetButton(CrouchInput) && CrouchingBlocked == false && !Input.GetButton(RunInput)) {
			TheX = MoveSpeed * Time.fixedDeltaTime * InputHor;  
			TheZ = MoveSpeed * Time.fixedDeltaTime * InputVert;
		}
		if (!Input.GetButton (CrouchInput) && CrouchingBlocked == false  && Input.GetButton(RunInput)) {
			TheX = RunSpeed * Time.fixedDeltaTime * InputHor;  
			TheZ = RunSpeed * Time.fixedDeltaTime * InputVert;

		}	
		if (Input.GetButton (CrouchInput) || CrouchingBlocked) {
				TheX = CrouchSpeed * Time.fixedDeltaTime * InputHor;  
				TheZ = CrouchSpeed * Time.fixedDeltaTime * InputVert;
		}

		if(Physics.Raycast(Controller.transform.position, Controller.transform.up, Controller.height *0.55f,HeadHitCheckLayers ))
		{
			tempJumpSpeed = 0;
		}

		tempJumpSpeed -= Gravity;
		TheY = Time.fixedDeltaTime * tempJumpSpeed ;
		

		if (HalfSpeed == false) {
			TheVector = new Vector3 (TheX, TheY, TheZ); 
		}
		if (HalfSpeed == true) {
			TheVector = new Vector3 (TheX * 0.707106682f, TheY, TheZ * 0.707106682f) ; //These numbers are very important. They prevent the controller from double diagonall acceleration (bug of older games where you can go faster by moving diagonally)
		}
			TheVector = Controller.transform.TransformDirection(TheVector); 
		Controller.Move (TheVector);
	}
}