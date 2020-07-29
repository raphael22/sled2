using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rocket {
  private int index;
  public string name;
  public float fuel = 20.0f;
  private float fuelDecrease = 0.002f;
  public float temperature = 0;
  private int[] temperatureThreshold = new int[] { 100, 200, 250 };
  private float temperatureDecrease = 0.04f;
  private float temperatureIncrease = 0.05f;
  public string status = "deactivated";
  public float value;
  public bool active = false;
  public bool oldActive = false;
  public bool stalled = false;
  public bool exploded = false;
  public UnityEngine.KeyCode key;
  private UnityEngine.UI.Text warning_fuel;
  private UnityEngine.UI.Text warning_temperature;
  private GameObject component;
  private Renderer renderer;
  private Transform shield;
  private Renderer shieldRenderer;

  public Rocket(string name, float value, UnityEngine.KeyCode key, int index) {
    this.name = name;
    this.value = value;
    this.key = key;
    this.index = index;
    warning_fuel = GameObject.Find("warning_fuel_" + index).GetComponent<Text>();
    warning_fuel.text = "";
    warning_temperature = GameObject.Find("warning_temperature_" + index).GetComponent<Text>();
    warning_temperature.text = "";
    component = GameObject.Find(name);
    renderer = component.GetComponent<Renderer>();

    shield = component.transform.Find("Capsule");
    shieldRenderer = shield.GetComponent<Renderer>();
  }

  public void ToggleStatus() {
    if (exploded || stalled) {
      return;
    }
    active = !active;
    status = active ? "activated" : "deactivated";
  }
  public void UpdateOnce() {
    if (active) {
      shieldRenderer.material.SetColor("_Color", Color.green);
      if (fuel > 0) {
        Move.rocketAcceleration = Move.rocketAcceleration + value;
      }
    } else {
      shieldRenderer.material.SetColor("_Color", Color.gray);
      Move.rocketAcceleration = Move.rocketAcceleration - value;
    }
  }
  public void Update() {
    if (active) {
      fuel = fuel - fuelDecrease;
      temperature = temperature + temperatureIncrease;
      if (fuel <= 0) {
        fuel = 0;
        active = false;
      }
    } else {
      if (temperature > 0) {
        temperature = temperature - temperatureDecrease;
      } else {
        temperature = 0;
      }
    }

    if (temperature > temperatureThreshold[0]) {
      if (exploded) {
        return;
      }
      if (temperature > temperatureThreshold[2]) {
        active = false;
        exploded = true;
        warning_temperature.text = name + " exploded ! \n";
        renderer.material.SetColor("_Color", Color.red);
      } else if (temperature > temperatureThreshold[1]) {
        warning_temperature.text = name + " is about to explode ! \n";
        renderer.material.SetColor("_Color", new Color(1, 0.6f, 0, 1));
      } else {
        warning_temperature.text = name + " is hot ! \n";
        renderer.material.SetColor("_Color", Color.yellow);
      }
    } else {
      warning_temperature.text = "";
      renderer.material.SetColor("_Color", Color.green);
    }

    if (fuel < 5) {
      if (stalled) {
        return;
      }
      if (fuel <= 0) {
        active = false;
        stalled = true;
        warning_fuel.text = name + " is stalled ! \n";
        renderer.material.SetColor("_Color", Color.red);
      } else if (fuel < 1) {
        warning_fuel.text = name + " is about to stall ! \n";
      } else {
        warning_fuel.text = name + " is low on fuel \n";
      }
    } else {
      warning_fuel.text = "";
    }
    if (active != oldActive) {
      oldActive = active;
      UpdateOnce();
    }
  }
}
