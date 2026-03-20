using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("──── Wheel Colliders ────")]
    public WheelCollider wheelFL;  
    public WheelCollider wheelFR;   
    public WheelCollider wheelRL;  
    public WheelCollider wheelRR;   

    [Header("──── Vizualna kolesa ────")]
    public Transform wheelMeshFL;
    public Transform wheelMeshFR;
    public Transform wheelMeshRL;
    public Transform wheelMeshRR;

    [Header("──── Nastavitve motorja ────")]
    public float motorForce = 1500f;        
    public float brakeForce = 3000f;        
    public float maxSteerAngle = 30f;    

    [Header("──── Realizem & občutek ────")]
    public float steerSpeed = 8f;        
    [Range(0.1f, 1f)] public float steerFalloff = 0.4f;  
    public float downforce = 15f;    

    private float currentSteerAngle = 0f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Manjka Rigidbody na avtu!");
        }
    }

    void FixedUpdate()
    {
        float motorInput = Input.GetAxisRaw("Vertical"); 
        float steerInput = Input.GetAxisRaw("Horizontal"); 

        float currentSpeed = rb.linearVelocity.magnitude * 3.6f; 

        //Motor
        float torque = motorInput * motorForce;

        wheelFL.motorTorque = torque;
        wheelFR.motorTorque = torque;
        wheelRL.motorTorque = torque;
        wheelRR.motorTorque = torque;

        // Zaviranje
        float brake = 0f;
        if (Input.GetKey(KeyCode.Space))
        {
            brake = brakeForce;
        }
        else if (motorInput < 0 && currentSpeed < 2f)
        {
            brake = brakeForce * 0.6f;
        }

        wheelFL.brakeTorque = brake;
        wheelFR.brakeTorque = brake;
        wheelRL.brakeTorque = brake;
        wheelRR.brakeTorque = brake;

        //           Volan
        float targetSteer = steerInput * maxSteerAngle;

        // manjši kot volana pri visoki hitrosti
        float speedFactor = Mathf.Clamp01(currentSpeed / 120f);
        targetSteer *= Mathf.Lerp(1f, steerFalloff, speedFactor);

        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteer, steerSpeed * Time.fixedDeltaTime);

        wheelFL.steerAngle = currentSteerAngle;
        wheelFR.steerAngle = currentSteerAngle;


        // Pritisk k tlom
        if (currentSpeed > 30)
        {
            Vector3 downForceVector = -transform.up * (currentSpeed * downforce * 0.1f);
            rb.AddForce(downForceVector);
        }

        UpdateWheelVisual(wheelFL, wheelMeshFL);
        UpdateWheelVisual(wheelFR, wheelMeshFR);
        UpdateWheelVisual(wheelRL, wheelMeshRL);
        UpdateWheelVisual(wheelRR, wheelMeshRR);
    }

    void UpdateWheelVisual(WheelCollider collider, Transform visual)
    {
        if (collider == null || visual == null) return;

        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);

        visual.position = pos;
        visual.rotation = rot;
    }

    // Pomožna funkcija za debug
    void OnGUI()
    {
        if (rb == null) return;

        float speed = rb.linearVelocity.magnitude * 3.6f;
        GUI.Label(new Rect(10, 10, 300, 30), $"Hitrost: {speed:F1} km/h");
    }
}