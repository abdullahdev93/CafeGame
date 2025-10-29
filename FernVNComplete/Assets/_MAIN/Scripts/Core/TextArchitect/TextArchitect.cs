using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;
using System.Linq;
using System;

public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_world;

    /// <summary>
    /// The assigned text component for this architect.
    /// </summary>
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    /// <summary>
    /// Font size property
    /// </summary>
    public float fontSize
    {
        get => tmpro.fontSize;
        set => tmpro.fontSize = value;
    }

    /// <summary>
    /// The text built by this architect.
    /// </summary>
    public string currentText => tmpro.text;
    public string targetText { get; private set; } = "";
    public string fullTargetText => preText + targetText;
    public string preText { get; private set; } = "";
    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }
    public float speed { get { return BASE_SPEED * speedMultiplier; } set { speedMultiplier = value; } }
    private const float BASE_SPEED = 1;
    private float speedMultiplier = 1;
    public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    private int characterMultiplier = 1;
    public bool hurryUp = false;
    private Dictionary<string, Type> builders = new Dictionary<string, Type>();
    private TABuilder builder = null;
    private TABuilder.BuilderTypes _builderType;
    public TABuilder.BuilderTypes builderType => _builderType;
    private Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null;

    public TextArchitect(TextMeshProUGUI uiTextObject, TABuilder.BuilderTypes builderType = TABuilder.BuilderTypes.Instant)
    {
        tmpro_ui = uiTextObject;
        AddBuilderTypes();
        SetBuilderType(builderType);
    }

    public TextArchitect(TextMeshPro worldTextObject, TABuilder.BuilderTypes builderType = TABuilder.BuilderTypes.Instant)
    {
        tmpro_world = worldTextObject;
        AddBuilderTypes();
        SetBuilderType(builderType);
    }

    private void AddBuilderTypes()
    {
        builders = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TABuilder)))
            .ToDictionary(t => t.Name, t => t);
    }

    public void SetBuilderType(TABuilder.BuilderTypes builderType)
    {
        string name = TABuilder.CLASS_NAME_PREFIX + builderType.ToString();
        Type classType = builders[name];

        builder = Activator.CreateInstance(classType) as TABuilder;
        builder.architect = this;
        builder.onComplete += OnComplete;

        _builderType = builderType;
    }

    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;

        Stop();

        // Ensure the correct font size is applied immediately before starting to build the text
        ApplyFontSize(); 

        // Ensure the font size is applied when building the text
        //tmpro.fontSize = fontSize;

        buildProcess = builder.Build();
        return buildProcess;
    }

    public Coroutine Append(string text)
    {
        preText = currentText;
        targetText = text;

        Stop();

        // Ensure the correct font size is applied immediately before appending text
        ApplyFontSize(); 

        // Ensure the font size is applied when appending the text
        //tmpro.fontSize = fontSize;

        buildProcess = builder.Build();
        return buildProcess;
    }

    public void SetText(string text)
    {
        preText = "";
        targetText = text;

        Stop();

        // Ensure the correct font size is applied immediately before setting the text
        ApplyFontSize(); 

        // Ensure the font size is applied when setting the text
        //tmpro.fontSize = fontSize;

        tmpro.text = targetText;
        builder.ForceComplete();
    }

    private void ApplyFontSize()
    {
        // Apply the font size before rendering text
        if (tmpro != null)
        {
            tmpro.fontSize = fontSize; // Ensure the font size is applied right away
        }
    } 

    public void Stop()
    {
        if (isBuilding)
            tmpro.StopCoroutine(buildProcess);

        buildProcess = null;
    }

    public void ForceComplete()
    {
        if (isBuilding)
            builder.ForceComplete();

        Stop();

        OnComplete();
    }

    private void OnComplete()
    {
        hurryUp = false;
        buildProcess = null;
    }
} 
