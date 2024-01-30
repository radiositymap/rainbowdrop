using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> shapes;
    public Action onBlockLanded;
    public Action onTouchSky;
    public Action onLevelChange;
    public GameObject sky;
    public GameObject walls;
    public GameObject gameOverMenu;
    public GameObject nextLevelMenu;
    public GameObject errorMenu;
    public GameObject winMenu;
    public Text levelText;
    public List<Color> colours;
    public int skyPos = 15;
    public int currLevel = 0;
    public bool pauseControls = false;
    public AudioSource gameOverSound;
    public AudioSource setBlockSound;
    public AudioSource errorSound;

    Dictionary<Vector2, Block> blockDict = new Dictionary<Vector2, Block>();

    void Start()
    {
        Physics2D.gravity = new Vector3(0, -5f, 0);
        sky.transform.position = new Vector3(0, skyPos, 0);
        onBlockLanded += OnBlockLanded;
        onTouchSky += GameOver;
        onLevelChange += ChangeLevel;
    }

    public void UnpauseControls() {
        pauseControls = false;
    }

    public void ResetGame() {
        BlockController[] blocks = FindObjectsOfType<BlockController>();
        foreach (BlockController block in blocks) {
            Destroy(block.gameObject);
        }
        sky.transform.position = new Vector3(0, skyPos, 0);
        currLevel = 0;
        if (onLevelChange != null)
            onLevelChange();
        DropBlock();
    }

    void DropBlock() {
        // drop block
        int blockId = UnityEngine.Random.RandomRange(0, shapes.Count);

        Color colour = Color.white;
        if (currLevel == 0)
            colour = colours[0];
        else if (currLevel == 1)
            colour = colours[UnityEngine.Random.Range(1, 3)];
        else if (currLevel == 2)
            colour = colours[UnityEngine.Random.Range(2, 4)];
        else if (currLevel >= 3)
            colour = colours[UnityEngine.Random.Range(2, 5)];

        GameObject block = Instantiate(shapes[blockId],
            new Vector3(0, 10, 0), Quaternion.identity);
        
        SpriteRenderer[] renderers = block.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers) {
            renderer.color = colour;
        }
    }

    void ChangeLevel() {
        sky.transform.position = new Vector3(0, skyPos, 0);
        BlockController[] blocks = FindObjectsOfType<BlockController>();
        foreach (BlockController block in blocks) {
            Destroy(block.gameObject);
        }
        levelText.text = "Level: " + currLevel;

        if (currLevel > 0)
            walls.GetComponent<Collider2D>().isTrigger = true;
        else
            walls.GetComponent<Collider2D>().isTrigger = false;

        if (currLevel == 4) {
            StartCoroutine(OpenErrorMenu());
        }
        if (currLevel > 4) {
            ResetGame();
            winMenu.SetActive(true);
        }
    }

    IEnumerator OpenErrorMenu() {
        while (currLevel == 4) {
            yield return new WaitForSeconds(15f);
            if (!errorMenu.activeInHierarchy) {
                errorSound.Play();
                errorMenu.SetActive(true);
                pauseControls = true;
            }
        }
    }

    void OnBlockLanded() {
        setBlockSound.Play();
        StartCoroutine(CheckForRows());
        StartCoroutine(DropNextBlockTask(1f));
    }

    void GameOver() {
        gameOverMenu.SetActive(true);
        gameOverSound.Play();
    }

    IEnumerator CheckForRows() {
        yield return new WaitForSeconds(1f);
        Block[] blocks = FindObjectsOfType<Block>();
        blockDict.Clear();
        foreach (Block block in blocks) {
            blockDict.Add(new Vector2(block.x, block.y), block);
        }
        List<int> matches = new List<int>();
        for (int i=-9; i<9; i++) {
            Color currCol = Color.black;
            for (int j=-8; j<8; j++) {
                Vector2 currPos = new Vector2(j, i);
                if (!blockDict.ContainsKey(currPos)) {
                    break;
                }
                Debug.Log("Check: " + i + " " + j + " " + currCol);
                if (j==-8)
                    currCol = blockDict[currPos].colour;
                else {
                    if (blockDict[currPos].colour != currCol)
                        break;
                }
                if (j==7) {
                    matches.Add(i);
                    Debug.Log("i " + i);
                }
            }
        }

        foreach (int match in matches) {
            Debug.Log("Found matches: " + match);

            for (int i=-8; i<8; i++) {
                Vector2 blockId = new Vector2(i, match);
                if (blockDict.ContainsKey(blockId)) {
                    Block block = blockDict[blockId];
                    Transform parent = block.transform.parent;
                    if (parent.childCount <= 0) {
                        Destroy(parent.gameObject);
                    }
                    else {
                        Destroy(block.gameObject);
                    }
                }
            }
            sky.transform.position += new Vector3(0, 1, 0);
        }
        if (matches.Count > 0) {
            nextLevelMenu.SetActive(true);
            currLevel += 1;
            onLevelChange();
        }
    }

    IEnumerator DropNextBlockTask(float pauseDuration) {
        yield return new WaitForSeconds(pauseDuration);
        DropBlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
