using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public Text taskText;
    public Text levelNumber;
    public Text changesNumber;
    public GameObject changesInput;
    public InputField changesInputField;
    public List<GameObject> branches;
    public GameObject succesModalWindow;
    public GameObject failModalWindow;
    public GameObject crossbarImg;
    public GameObject helpBranch;
    public GameObject nextTaskBtn;
    public Text finalText;

    List<GameObject> allBranches;
    RectTransform helpBranchRect;

    int level;
    Task[] tasks;
    PlayGround playground;
    int moves;


    void Start()
    {
        level = 1;
        tasks = new Task[]
        {
        new Task(1, 3),
        new Task(2, 4),
        new Task(3, 5),
        new Task(4, 4),
        new Task(5, 5),
        new Task(6, 6),
        new Task(7, 4),
        new Task(8, 5),
        new Task(9, 6)
        };
        moves = 0;
        helpBranchRect = (RectTransform)helpBranch.transform;
        helpBranch.SetActive(false);
        NewLevel();
    }

    void NewLevel()
    {
        playground = new PlayGround(tasks[level - 1]);
        DrawLevel();
    }

    void DrawLevel()
    {
        levelNumber.text = level + "";
        taskText.text = playground.task.GetText(level);
        succesModalWindow.SetActive(false);
        failModalWindow.SetActive(false);
        DrawBranches();
        if(level < 4)
        {
            changesInput.SetActive(false);
            changesNumber.text = "0";
        }
        else
        {
            changesInput.SetActive(true);
            changesInputField.text = "";
            changesNumber.text = "";
        }
    }

    void DrawBranches()
    {
        allBranches = new List<GameObject>();
        float initX = helpBranchRect.transform.position.x;
        float initY = helpBranchRect.transform.position.y;
        float width = helpBranchRect.rect.width;
        for (int i = 0; i < playground.playground.Length; i++) {
            GameObject branch = Instantiate(branches[playground.playground[i]], new Vector3(initX + i*(width+10), initY, 0), Quaternion.identity);
            branch.transform.SetParent(gameObject.transform);
            allBranches.Add(branch);
        }
        crossbarImg.transform.SetAsLastSibling();
        Debug.Log("all=" + allBranches.Count);
    }

    public RectTransform GetHelpBranchRect()
    {
        return helpBranchRect;
    }

    public List<GameObject> GetAllBranches()
    {
        return allBranches;
    }

    public void IncreaseMoves()
    {
        moves++;
        if (level < 4)
        {
            changesNumber.text = moves + "";
        }
    }

    public void ResetGame()
    {
        for(int i = 0; i < GetAllBranches().Count; i++)       
        {
            GameObject brancha = GetAllBranches()[i];
            GetAllBranches()[i] = null;
            Destroy(brancha);
        }
        moves = 0;
        DrawLevel();
        playground.ResetPlayerSolution();
    }

    public void ResetAfterFail()
    {
        ResetGame();
        failModalWindow.SetActive(false);
    }

    public bool Check()
    {
        if(level < 4)
        {
            int[] sol = playground.playerSolutionList;
            for(int i = 0; i < playground.solutionList.Length; i++)
            {
                if(sol[i] != playground.solutionList[i])
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return Int32.Parse(changesInputField.text) == playground.solutionNum;
        }
    }

    public void OkBtnClick()
    {
        if (Check())
        {
            succesModalWindow.SetActive(true);
            succesModalWindow.transform.SetAsLastSibling();
            if(level == 9)
            {
                nextTaskBtn.SetActive(false);
                finalText.text = "Gratulujem! Vyriešil si všetky úlohy.";
            }
        }
        else
        {
            failModalWindow.SetActive(true);
            failModalWindow.transform.SetAsLastSibling();
        }
    }

    public void SwitchIndexes(int i1, int i2)
    {
        playground.SwitchIndexes(i1, i2);
    }

    public int GetLevel()
    {
        return level;
    }

    public void NextLevel()
    {
        if (level < 9)
        {
            level++;
            moves = 0;
            succesModalWindow.SetActive(false);
            ResetGame();
            NewLevel();
            DrawLevel();
        }
    }

    class Task
    {
        public int level;
        public string taskText;
        public int countBranches;

        public Task(int level, int countBranches)
        {
            this.level = level;
            this.countBranches = countBranches;
            this.taskText = GetText(level);
        }

        public string GetText(int level)
        {
            if (level < 4)
            {
                return "Usporiadaj brvná podľa počtu lístkov tak, aby pri bobrovi bolo brvno s najväčším počtom lístkov. Vymieňať môžeš iba brvná ležiace vedľa seba. Všímaj si počet výmen. ";

            }
            else if(level < 7)
            {
                return "Na aký najmenší počet výmen sa dajú postupne usporiadať brvná tak, aby pri bobrovi bolo brvno s najväčším počtom lístkov? Pomôž si usporiadavaním vetvičiek a riešenie napíš.";
            }
            else
            {
                return "Na aký najmenší počet výmen sa dajú postupne usporiadať brvná tak, aby pri bobrovi bolo brvno s najväčším počtom lístkov? Teraz si už nemôžeš pomôcť usporiadavaním vetvičiek.";
            }
        }
    }

    class PlayGround
    {
        public Task task;
        public int[] playground;
        public int solutionNum;
        public int[] solutionList;
        public int[] playerSolutionList;

        public PlayGround(Task task)
        {
            this.task = task;
            this.playground = GeneratePlayground(task.countBranches);
            this.solutionList = GetSolutionList(task.countBranches);
            this.solutionNum = GetSolutionNum();
            this.playerSolutionList = (int[])playground.Clone();
        }

        public int[] GeneratePlayground(int countBranches)
        {
            int[] list = new int[countBranches];
            for (int i = 0; i < countBranches; i++)
            {
                list[i] = i;
            }
            Shuffle(list);
            while (list[0] == countBranches - 1)
            {
                Shuffle(list);
            }
            return list;
        }

        public void ResetPlayerSolution()
        {
            this.playerSolutionList = (int[])playground.Clone();
        }

        public void Shuffle(int[] array)
        {
            System.Random _random = new System.Random();
            int p = array.Length;
            for (int n = p - 1; n > 0; n--)
            {
                int r = _random.Next(0, n);
                int t = array[r];
                array[r] = array[n];
                array[n] = t;
            }
        }

        public int[] GetSolutionList(int countBranches)
        {
            int[] sol = new int[countBranches];
            for (int i = 0; i < countBranches; i++)
            {
                sol[i] = countBranches - i - 1;
            }
            return sol;
        }

        public int GetSolutionNum()
        {
            Solver solver = new Solver();
            int[] reverse = new int[playground.Length];
            for(int i = 0; i < playground.Length; i++)
            {
                reverse[i] = playground[playground.Length - i - 1];
            }
            int solution = solver.CountSwaps(reverse, playground.Length);
            Debug.Log(solution);
            return solution;
        }

        public void SwitchIndexes(int i1, int i2)
        {
            int val = this.playerSolutionList[i1];
            this.playerSolutionList[i1] = this.playerSolutionList[i2];
            this.playerSolutionList[i2] = val;

        }
    }

    class Solver
    {
        int Merge(int[] arr, int[] temp, int left, int mid, int right)
        {
            int inv_count = 0;
            int i = left;
            int j = mid;
            int k = left;

            while ((i <= mid - 1) &&
                (j <= right))
            {
                if (arr[i] <= arr[j])
                    temp[k++] = arr[i++];
                else
                {
                    temp[k++] = arr[j++];
                    inv_count = inv_count + (mid - i);
                }
            }

            while (i <= mid - 1)
                temp[k++] = arr[i++];
            while (j <= right)
                temp[k++] = arr[j++];
            for (i = left; i <= right; i++)
                arr[i] = temp[i];

            return inv_count;
        }

        int _mergeSort(int[] arr, int[] temp, int left, int right)
        {
            int mid, inv_count = 0;
            if (right > left)
            {
                mid = (right + left) / 2;
                inv_count = _mergeSort(arr, temp, left, mid);
                inv_count += _mergeSort(arr, temp, mid + 1, right);
                inv_count += Merge(arr, temp, left, mid + 1, right);
            }
            return inv_count;
        }

        public int CountSwaps(int[] arr, int n)
        {
            int[] temp = new int[n];
            return _mergeSort(arr, temp, 0, n - 1);
        }

    } 

 

    
}
