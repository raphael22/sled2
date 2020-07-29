using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Move : MonoBehaviour {

  private float acceleration = 0;
  private float initialAcceleration = 0.01f;
  public static float rocketAcceleration = 0;
  private float deceleration = 0;
  private float decelerationMax = 0.08f;
  private float decelerationMin = 0.04f;
  private float decelerationInitial = 0.04f;
  private float velocity;
  private Rocket[] rockets;
  public GameObject player, finish;
  Collider player_collider, finish_collider;
  private float timer = 0;
  private bool done = false;


  // Start is called before the first frame update
  void Start() {
    rockets = new Rocket[4] {
      new Rocket("Rocket_1", 0.01f, KeyCode.F1, 1),
      new Rocket("Rocket_2", 0.01f, KeyCode.F2, 2),
      new Rocket("Rocket_3", 0.01f, KeyCode.F3, 3),
      new Rocket("Rocket_4", 0.01f, KeyCode.F4, 4)
    };

    player_collider = player.GetComponent<Collider>();
    finish_collider = finish.GetComponent<Collider>();
    Debug.Log(finish_collider.bounds);
  }

  void RocketLoop() {
    for (int id = 0; id < rockets.Length; id++) {
      var rocket = rockets[id];
      if (Input.GetKeyDown(rocket.key)) {
        rocket.ToggleStatus();
      }
      if (rocketAcceleration < 0) {
        rocketAcceleration = 0;
      }
      rocket.Update();
    }
  }

  void UiLoop() {
    var velocityUi = GameObject.Find("Velocity").GetComponent<Text>();
    velocityUi.text = Mathf.Round(velocity * 200).ToString() + "Km/h";

    var rocketsUi = GameObject.Find("Rockets").GetComponent<Text>();
    var rocketsUiText = "";
    for (int id = 0; id < rockets.Length; id++) {
      var rocket = rockets[id];
      rocketsUiText = rocketsUiText + "; " + rocket.name + ": " + rocket.status + " fuel: " + rocket.fuel + " temperature: " + rocket.temperature + "\n";
    }
    rocketsUi.text = rocketsUiText;
  }

  void DecelerationLoop() {

    deceleration = velocity * 0.1f;
    if (deceleration > decelerationMax) {
      deceleration = decelerationMax;
    }
    if (deceleration < decelerationMin) {
      deceleration = decelerationMin;
    }
    if (Input.GetKey(KeyCode.DownArrow)) {
      deceleration = deceleration + decelerationInitial;
    }
    deceleration = deceleration - (rocketAcceleration * 1.8f);
    if (deceleration < 0) {
      deceleration = 0;
    }
  }

  void VelocityLoop() {

    if (Input.GetKey(KeyCode.UpArrow)) {
      acceleration = initialAcceleration;
    } else {

      if (velocity > 0) {
        velocity = velocity - (Time.deltaTime * deceleration);
      }
      acceleration = 0;
    }

    velocity = velocity + (Time.deltaTime * (acceleration + rocketAcceleration));
    if (velocity < 0) {
      velocity = 0;
    }

    if (player_collider.bounds.Intersects(finish_collider.bounds)) {
      Debug.Log("collide");
      velocity = 0;
      done = true;
    }
    gameObject.transform.Translate(0, 0, velocity);

  }

  // Update is called once per frame
  void Update() {
    RocketLoop();

    DecelerationLoop();

    VelocityLoop();

    UiLoop();

    if (!done) {
      timer += Time.deltaTime;
      var Timer = GameObject.Find("Timer").GetComponent<Text>();
      Timer.text = System.Math.Round(timer, 2).ToString();
    }

    // Debug.Log("velocity: " + velocity + " acceleration: " + acceleration + " deceleration: " + deceleration + " rocketV: " + rocketAcceleration);
  }

}
