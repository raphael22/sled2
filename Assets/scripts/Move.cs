using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Move : MonoBehaviour {

  private float acceleration = 0;
  private float initialAcceleration = 0.01f;
  public static float rocketAcceleration = 0;
  private float deceleration = 0;
  private float decelerationMax = 0.1f;
  private float decelerationMin = 0.04f;
  private float decelerationInitial = 0.04f;
  private float velocity;
  private Rocket[] rockets;
  public GameObject player, finish;
  Collider player_collider, finish_collider;
  private float distance, highscore;
  private bool started = false, done = false;
  private int ratio = 100;


  // Start is called before the first frame update
  void Start() {
    rockets = new Rocket[4] {
      new Rocket("Rocket_1", 0.01f, KeyCode.Alpha1, 1),
      new Rocket("Rocket_2", 0.01f, KeyCode.Alpha2, 2),
      new Rocket("Rocket_3", 0.01f, KeyCode.Alpha3, 3),
      new Rocket("Rocket_4", 0.01f, KeyCode.Alpha4, 4)
    };

    player_collider = player.GetComponent<Collider>();
    finish_collider = finish.GetComponent<Collider>();
    Debug.Log(finish_collider.bounds);
    highscore = PlayerPrefs.GetFloat("highscore");
    GameObject.Find("Highscore").GetComponent<Text>().text = "HighScore: " + System.Math.Round(highscore).ToString();
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

    var rocketsUi = GameObject.Find("Rockets").GetComponent<Text>();
    var rocketsUiText = "";
    for (int id = 0; id < rockets.Length; id++) {
      var rocket = rockets[id];
      rocketsUiText = rocketsUiText + "; " + rocket.name + ": " + rocket.status + " fuel: " + rocket.fuel + " temperature: " + rocket.temperature + "\n";
    }
    rocketsUi.text = rocketsUiText;

    var newDistance = (player.transform.position.z);
    var velocityHour = ((newDistance - distance) * ratio) * 3.6f;
    velocityUi.text = Mathf.Round(velocityHour).ToString() + "Km/h";
    distance = newDistance;
    var Distance = GameObject.Find("Distance").GetComponent<Text>();
    Distance.text = System.Math.Round(distance).ToString() + " meter";
  }

  void DecelerationLoop() {

    deceleration = velocity * 0.12f;
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

    gameObject.transform.Translate(0, 0, velocity);
  }

  // Update is called once per frame
  void Update() {
    if (!done) {

      CheckScore();

      RocketLoop();

      DecelerationLoop();

      VelocityLoop();

      UiLoop();
    }

    // Debug.Log("velocity: " + velocity + " acceleration: " + acceleration + " deceleration: " + deceleration + " rocketV: " + rocketAcceleration);
  }

  void CheckScore() {
    if (started && velocity < 0f) {
      Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!" + (started && velocity < 0f));
      velocity = 0f;
      started = false;
      done = true;
    }
    if (velocity > 0.1f) {
      started = true;
    }
    Debug.Log("velocity: " + velocity + " started " + started + " done " + done);

    if (done) {
      velocity = 0;
      started = false;
      if (highscore <= 0 || distance > highscore) {
        highscore = distance;
        PlayerPrefs.SetFloat("highscore", highscore);
        Debug.Log("highscore: " + highscore);
        GameObject.Find("Highscore").GetComponent<Text>().text = "HighScore: " + System.Math.Round(highscore).ToString();
      }
    }
  }

}
