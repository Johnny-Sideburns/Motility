using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CurveMovePlayer : MonoBehaviour
{
    public float _timer;
    [SerializeField] private float _slideValue;
    [SerializeField] private float _timeMax;
    [SerializeField] private float _snapSpeed;
    [SerializeField] private List<MoveData> _moveDatas;
    public CurveMoverContext _context;
    private bool _playing;
    private List<CurveMover> _moves;
    private bool _isDone;
    public bool IsDone => _isDone;
    public float TimeMax => _timeMax;
    public int Reverse = 1;

    void Awake()
    {
        _moves = new List<CurveMover>();
        foreach (MoveData moveData in _moveDatas)
        {
            _moves.Add(_context.GetMover(moveData.mover));
        }
    }
    void Update()
    {
        //Hackey input listening for shifting weights
        int key = -1;

        if (Keyboard.current.digit1Key.isPressed) key = 1;
        else if (Keyboard.current.digit2Key.isPressed) key = 2;
        else if (Keyboard.current.digit3Key.isPressed) key = 3;

        if (key != -1)
        {
            Slide(key);
        }


        if (!_playing) return;
        _timer += Time.deltaTime * Reverse;
        if (_timer > _timeMax)
        {   
            _timer = _timeMax;
            _isDone = true;
        }
        if (_timer < 0)
        {
            _timer = 0;
        }
        _moves.ForEach(x => x.SetTimeX(_timer/_timeMax));

    }

    public void Prepare()
    {
        foreach (MoveData moveData in _moveDatas)
        {
            _context.GetMover(moveData.mover).SetMoveData(moveData.weightedCurvedata);
            _context.GetMover(moveData.mover).SetSnapSpeed(_snapSpeed);

        }
    }
    public void Slide(int numberPressed)
    {
        Debug.Log("number pressed: " + numberPressed);
        foreach (MoveData moveData in _moveDatas)
        {
            int count = moveData.weightedCurvedata.Count;
            Debug.Log("count: " + count);

            if (count <= 1 || numberPressed > count) continue;
            Debug.Log("start loop: " + numberPressed);

            for (int i = 0; i < count; i++)
            {
                float value = moveData.weightedCurvedata[i].weight;
                if (i == numberPressed -1)
                {
                    Debug.Log("increasing index: " + i);

                    value += Time.deltaTime * _slideValue;
                    moveData.weightedCurvedata[i].weight = Mathf.Min(value, 1);
                } 
                else
                {
                    Debug.Log("decreasing index: " + i);
                    value -= Time.deltaTime * _slideValue/(count-1);
                    moveData.weightedCurvedata[i].weight = Mathf.Max(value, 0);
                }
            }
        }
        Prepare();
    }
    public void Go()
    {
        _playing = true;
    }
    public void Hold()
    {
        _moves.ForEach(x => x.Hold());
    }
    public void UnHold()
    {
        _moves.ForEach(x => x.UnHold()); 
    }
    public void Stop()
    {
        _playing = false;
    }
    public void ResetValues()
    {
        _timer = 0;
        _isDone = false;
    }
}
