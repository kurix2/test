using UnityEngine;
using UnityEngine.SceneManagement;


public class Rocket : MonoBehaviour {

    // todo: fix lighting bug


    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;


    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    [SerializeField]
    enum State
    {
        Alive,
        Dying,
        Transcending
    };


    State state = State.Alive;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

        if (state == State.Alive){
            RespondToThrustInput();
            RespondeToRotateInput();
        } 

	}

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        switch(collision.gameObject.tag) {
            case "Friendly":
                break;
            case "Dead":
                StartDeathSequence();
                break;
            default:
                StartSuccessSequence();
                break;


        }
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay); // parameterize time
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        mainEngineParticles.Stop();
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();

        Invoke("HideChildren", 0.1f);      
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void RespondeToRotateInput() {

        rigidBody.freezeRotation = true; //

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }
     
    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }

        mainEngineParticles.Play();
    }

    private void HideChildren(){
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rs)
        {
            if (r != GameObject.Find("Explosion Particles").GetComponent<Renderer>())
            r.enabled = false;
        }
    }
}
