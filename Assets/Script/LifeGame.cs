using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class LifeGame : MonoBehaviour
{
    public Material Living;

    public Material Dead;

    public GameObject Cell;

    /// <summary>
    /// MeshRenderer配列の横の数
    /// </summary>
    private int Width = 80;

    /// <summary>
    /// MeshRenderer配列の縦の数
    /// </summary>
    private int Height = 50;

    /// <summary>
    /// 参照用、作業用セルの横のサイズ
    /// </summary>
    private int RefWidth;

    /// <summary>
    /// 参照用、作業用セルの縦のサイズ
    /// </summary>
    private int RefHeight;

    /// <summary>
    /// MeshRenderer配列
    /// </summary>
    private MeshRenderer[] CellsMR;

    /// <summary>
    /// 参照用セル配列
    /// </summary>
    private bool[] RefCells;

    /// <summary>
    /// 作業用セル配列
    /// </summary>
    private bool[] WCells;
    
    // Start is called before the first frame update
    void Start()
    {
        AllocArray();
        InitArray();
        StartCoroutine(nameof(GameStart));

    }

    /// <summary>
    /// 配列の初期化
    /// </summary>
    private void InitArray()
    {
        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                int n = h * Width + w;
                var position = new Vector3(w + 0.5f, 0, h + 0.5f);
                this.CellsMR[n] = ((GameObject) Instantiate(Cell, position, Quaternion.identity))
                    .GetComponent<MeshRenderer>();
            }
        }
    }

    public IEnumerator GameStart()
    {
        while (true)
        {
            Next();
            RenewCells();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void RenewCells()
    {
        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                int n = h * Width + w;
                int p = (h + 1) * RefWidth + (w + 1);

                if (RefCells[p])
                {
                    CellsMR[n].material = Living;
                }
                else
                {
                    CellsMR[n].material = Dead;
                }
            }
        }
    }

    /// <summary>
    /// 次世代のセルを求め、更新する
    /// </summary>
    private void Next()
    {
        for (int h = 1; h < this.RefHeight - 1; h++)
        {
            for (int w = 1; w < RefWidth - 1; w++)
            {
                int pos = h * RefWidth + w;
                int sum = SumAround(w, h);
                if (sum == 3)
                {
                    WCells[pos] = true;
                }
                else if (sum == 2)
                {
                    WCells[pos] = RefCells[pos];
                }
                else
                {
                    WCells[pos] = false;
                }
            }
        }
        WCells.CopyTo(RefCells,0);
        Array.Clear(WCells,0,WCells.Length);
    }

    /// <summary>
    /// 周囲8近傍の生きているセルの数を求める
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    private int SumAround(int w, int h)
    {
        int sum = 0;

        int up = (h + 1) * RefWidth;
        int ce = h * RefWidth;
        int bo = (h - 1) * RefWidth;

        sum += BoolToInt(RefCells[up + (w - 1)]);
        sum += BoolToInt(RefCells[up + w]);
        sum += BoolToInt(RefCells[up + (w + 1)]);
        sum += BoolToInt(RefCells[ce + (w - 1)]);
        sum += BoolToInt(RefCells[ce + w]);
        sum += BoolToInt(RefCells[ce + (w + 1)]);
        sum += BoolToInt(RefCells[bo + (w - 1)]);
        sum += BoolToInt(RefCells[bo + w]);
        sum += BoolToInt(RefCells[bo + (w + 1)]);
        return sum;
    }

    private int BoolToInt(bool b)
    {
        return (b ? 1 : 0);
    }

    /// <summary>
    /// 配列の確保
    /// </summary>
    private void AllocArray()
    {
        int size = Width * Height;
        RefWidth = Width + 2;
        RefHeight = Height + 2;
        int refSize = RefWidth * RefHeight;
        CellsMR = new MeshRenderer[size];
        RefCells = new bool[refSize];
        WCells = new bool[refSize];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RandomDot();
        }
    }

    private void RandomDot()
    {
        int dotNum = Width * Height / 4;
        for (int n = 0; n < dotNum; n++)
        {
            int px = UnityEngine.Random.Range(0, Width);
            int py = UnityEngine.Random.Range(0, Height);
            int p = (py + 1) * RefWidth + (px + 1);
            RefCells[p] = true;
        }
        this.RenewCells();
    }
}
