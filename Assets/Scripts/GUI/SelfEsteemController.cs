﻿using UnityEngine;
using System.Collections;

public class SelfEsteemController : MonoBehaviour
{

    public GameObject player;
    public ControllerScript controller;
    public Shader shaderFlashWhite;

    private float health = 10.0f;
    public float HPVal { get { return health; } set { health = value; if (health > 100.0) { health = 100.0f; } else if (health < 0.0f) { health = 0.0f; } } }

    //Shader Stuff
    private SpriteRenderer spriteRenderer;
    private float flashLength = 1.0f;
    private float flashTimer;
    private float flashAmount;
    private Color selfEsteemColor;

    private Vector3 localStartingPos;
    private Bounds startingBounds;

    //Face
    private GameObject esteemFace;
    private SpriteRenderer faceSpriteRenderer;
    private Animator faceAnimator;


    //JUICY stuff
    private float juiceTimer;
    private float bounceValue;
    private float actualBounce;
    public float Bounce { get { return bounceValue; } set { bounceValue = value; if (bounceValue > 100.0) { bounceValue = 100.0f; } else if (bounceValue < 0.0f) { bounceValue = 0.0f; } flashTimer = 0.3f; } }

    // Use this for initialization
    void Start()
    {
        //Pre-activation setup
        localStartingPos = this.transform.localPosition;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        esteemFace = this.transform.FindChild("EsteemFaces").gameObject;
        faceSpriteRenderer = esteemFace.GetComponent<SpriteRenderer>();
        faceAnimator = esteemFace.GetComponent<Animator>();
        startingBounds = spriteRenderer.bounds;

        //Activate
        activate();

        //White Flash Shader
        spriteRenderer.material.shader = shaderFlashWhite;
        faceSpriteRenderer.material.shader = shaderFlashWhite;
        selfEsteemColor = new Color(.454f, 1.0f, .070f, 1.0f);
        spriteRenderer.material.color = selfEsteemColor;
    }

    // Update is called once per frame
    void Update()
    {

        //DEBUG KEYCODES
        if (Input.GetKeyDown(KeyCode.O))
        {
            activate();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            deactivate();
        }
        //END DEBUG

        //Check if we are flashing
        if (flashTimer > 0.0f)
        {
            flashTimer -= Time.deltaTime;
            flashAmount = (flashTimer / flashLength);
            spriteRenderer.material.SetFloat("_FlashAmount", flashAmount);
            faceSpriteRenderer.material.SetFloat("_FlashAmount", flashAmount);

        }
        spriteRenderer.material.color = TransformHSV(selfEsteemColor, 110.0f - (health * 1.1f), 1.0f, 1.0f);
        //Update the Face
        switch ((int)(health / 10.0f))
        {
            case 10:
            case 9:
            case 8:
                faceAnimator.SetInteger("Emotion", 4);
                break;
            case 7:
            case 6:
                faceAnimator.SetInteger("Emotion", 3);
                break;
            case 5:
            case 4:
                faceAnimator.SetInteger("Emotion", 2);
                break;
            case 3:
            case 2:
            case 1:
                faceAnimator.SetInteger("Emotion", 1);
                break;
            case 0:
                faceAnimator.SetInteger("Emotion", 0);
                break;
            default:
                print("Emotion Switch Defaulted");
                break;
        }

        //JUICY JUICY JUICY 

        //Rotation
        juiceTimer += Time.deltaTime;
        float bounceValueMinimum = health / 10f;
        if (bounceValue > bounceValueMinimum)
        {
            bounceValue -= (110f - health / 2f) / 200f;
        }
        else
        {
            bounceValue = bounceValueMinimum;
        }
        actualBounce = Mathf.Lerp(actualBounce, bounceValue, 0.6f);
        this.transform.Rotate(Vector3.forward, (0.07f) * Mathf.Sin((3.0f * juiceTimer) + (Mathf.PI / 2)));

        //Bounce
        this.transform.localScale = Vector2.one * (2.7f + Mathf.Abs(((actualBounce / 100.0f) / 2f) * Mathf.Sin((3.0f * juiceTimer) + (Mathf.PI / 2))));

        //Change the pivot of the bounce.
        this.transform.localPosition = new Vector3(localStartingPos.x - (startingBounds.size.x / 2) + (spriteRenderer.bounds.size.x / 2), localStartingPos.y - (startingBounds.size.y / 2) + (spriteRenderer.bounds.size.y / 2), localStartingPos.z);

    }
    /// <summary>
    ///  Does a fancy enable animation
    /// </summary>
    public void activate()
    {
        spriteRenderer.enabled = true;
        faceSpriteRenderer.enabled = true;
        flashTimer = flashLength;

    }
    /// <summary>
    /// Disables the banner
    /// </summary>
    public void deactivate()
    {
        spriteRenderer.enabled = false;
        faceSpriteRenderer.enabled = false;

    }
    /// <summary>
    /// Hue shifting code. Stolen from http://beesbuzz.biz/code/hsv_color_transforms.php
    /// </summary>
    /// <param name="color"></param>
    /// <param name="H"></param>
    /// <param name="S"></param>
    /// <param name="V"></param>
    /// <returns>Hue Shifted Color</returns>
    private Color TransformHSV(
        Color color,  // color to transform
        float H,          // hue shift (in degrees)
        float S,          // saturation multiplier (scalar)
        float V           // value multiplier (scalar)
        )
    {
        float VSU = V * S * Mathf.Cos(H * Mathf.PI / 180);
        float VSW = V * S * Mathf.Sin(H * Mathf.PI / 180);

        Color ret = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        ret.r = (.299f * V + .701f * VSU + .168f * VSW) * color.r
            + (.587f * V - .587f * VSU + .330f * VSW) * color.g
                + (.114f * V - .114f * VSU - .497f * VSW) * color.b;
        ret.g = (.299f * V - .299f * VSU - .328f * VSW) * color.r
            + (.587f * V + .413f * VSU + .035f * VSW) * color.g
                + (.114f * V - .114f * VSU + .292f * VSW) * color.b;
        ret.b = (.299f * V - .3f * VSU + 1.25f * VSW) * color.r
            + (.587f * V - .588f * VSU - 1.05f * VSW) * color.g
                + (.114f * V + .886f * VSU - .203f * VSW) * color.b;
        return ret;
    }
}
