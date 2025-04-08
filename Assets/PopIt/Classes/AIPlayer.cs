using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopIt
{
    public class AIPlayer : MonoBehaviour
    {
        /// <summary>
        /// The mastermind that plays as the AI.
        /// The steps are simple amd can be checked via "PlayDelayed" method.
        /// </summary>

        public static AIPlayer instance;

        public List<BoardData> boardData;

        //Decision making arrays
        public List<int> rowWeight;         //we give each row a weight for importance. 

        //AI's selected buttons
        public List<GameObject> selectedButtons;


        private void Awake()
        {
            instance = this;

            boardData.Clear();
            rowWeight.Clear();
            selectedButtons.Clear();
        }

        void Start()
        {

        }

        void Update()
        {

        }

        /// <summary>
        /// To make the game realistic, we need to add a bit of delay for AI to play
        /// </summary>
        public void Play()
        {
            //Invoke("PlayDelayed", 2.0f);

            StartCoroutine(PlayDelayedCo());
        }

        public IEnumerator PlayDelayedCo()
        {
            print("<b>AI is playing</b>");

            //#1 - Init data arrays & create board data
            UpdateBoardDetailsOnAI();

            yield return new WaitForSeconds(0.2f);

            //#2 - Find best row to play based on calculated weight
            int bestRow = FindBestRow();

            yield return new WaitForSeconds(0.2f);

            //#3 - Find proper buttons to play
            FindProperButtons(bestRow);

            yield return new WaitForSeconds(0.2f);

            //#4 - make the move & end the turn
            if (selectedButtons.Count > 0)
            {
                //selectedButtons[0].GetComponent<ButtonManager>().ButtonClicked();
                StartCoroutine(SelectButtonsCo());
            }
        }

        /// <summary>
        /// AI monitors the whole board to asses the rows find the buttons that are playable.
        /// It also gives weight to each row, ie, some sort of importance based on the number of playable buttons in each row.
        /// </summary>
        public void UpdateBoardDetailsOnAI()
        {
            int rw = 0;          //tmp row weight
            boardData.Clear();
            rowWeight.Clear();

            for (int i = 0; i < BoardManager.instance.boardSize.x; i++) // BoardManager.instance.boardSize.x가 아니라 BoardManager.instance.boardSize.y??
            {
                boardData.Add(new BoardData());
                boardData[i].rowData = new List<bool>();

                //Reset
                rw = 0;

                for (int j = 0; j < BoardManager.instance.boardSize.y; j++)
                {
                    //if (BoardManager.instance.boardButtons[(i * (int)BoardManager.instance.boardSize.x) + j].GetComponent<ButtonManager>().buttonState == ButtonManager.ButtonStates.Finalized)
                    if (BoardManager.instance.boardButtons[(i * (int)BoardManager.instance.boardSize.y) + j].GetComponent<ButtonManager>().buttonState == ButtonManager.ButtonStates.Finalized)
                    {
                        boardData[i].rowData.Add(true);

                    }
                    else
                    {
                        boardData[i].rowData.Add(false);
                        rw++;
                    }
                }

                //Apply weight to each row
                rowWeight.Add(rw);
            }
        }


        /// <summary>
        /// AI selects a button
        /// </summary>
        /// <returns></returns>
        public IEnumerator SelectButtonsCo()
        {
            foreach (GameObject g in selectedButtons)
            {
                g.GetComponent<ButtonManager>().ButtonClicked();
                yield return new WaitForSeconds(0.35f);
            }

            yield return new WaitForSeconds(0.7f);
            EndTurn();
        }


        /// <summary>
        /// AI ends its turn
        /// </summary>
        public void EndTurn()
        {
            GameManager.instance.TurnManager();

            //Refresh data after new changes
            UpdateBoardDetailsOnAI();
            //selectedButtons.Clear();
        }


        /// <summary>
        /// We count total number of selectable buttons in each row. Row with the most number of playable buttons (highest weight) is considered a better option to play for AI
        /// </summary>
        /// <returns></returns>
        public int FindBestRow()
        {
            int bestRowID = -1;
            List<int> bestRowIDs = new List<int>();
            int highestWeight = 0;

            for (int i = 0; i < BoardManager.instance.boardSize.x; i++)
            {
                int rw = rowWeight[i];
                if (rw > highestWeight)
                {
                    bestRowIDs.Clear();
                    highestWeight = rw;
                    bestRowIDs.Add(i);
                }
                else if (rw == highestWeight)
                {
                    bestRowIDs.Add(i);
                }
            }

            bestRowID = bestRowIDs[Random.Range(0, bestRowIDs.Count)];

            print("bestRowIDs.Count: " + bestRowIDs.Count);
            print("BestRowID/Weight: " + bestRowID + "/" + highestWeight);

            return bestRowID;
        }


        /// <summary>
        /// AI find one or more buttons to play based on the given weight.
        /// The button with the most links has the biggest weight.
        /// </summary>
        /// <param name="selectedRow"></param>
        public void FindProperButtons(int selectedRow)
        {
            selectedButtons.Clear();

            GameObject bestStartingButton = null;
            List<GameObject> bestStartingButtons = new List<GameObject>();
            int bestStartingButtonIndex = -1;
            int biggestLink = 0;

            for (int i = 0; i < BoardManager.instance.boardSize.y; i++)
            {
                GameObject targetButton = BoardManager.instance.GetBoardButton((int)(selectedRow * BoardManager.instance.boardSize.y) + i);
                int btnLinks = targetButton.GetComponent<ButtonManager>().buttonLinks;
                if (btnLinks > biggestLink)
                {
                    bestStartingButtons.Clear();
                    biggestLink = btnLinks;
                    bestStartingButtonIndex = i;
                    bestStartingButtons.Add(targetButton);
                }
                else if (btnLinks == biggestLink)
                {
                    bestStartingButtons.Add(targetButton);
                }
            }

            print("bestStartingButtons.Count: " + bestStartingButtons.Count);
            for (int j = 0; j < bestStartingButtons.Count; j++)
            {
                print("bestStartingButtons #" + j + " => " + bestStartingButtons[j].name);
            }

            // 최적의 후보중 임의의 객체를 최적의 단 하나의 버튼으로 대입
            bestStartingButton = bestStartingButtons[Random.Range(0, bestStartingButtons.Count)];
            print("bestStartingButton: " + bestStartingButton.name);
            selectedButtons.Add(bestStartingButton);

            //Now check if neighbouring buttons can also be selected by AI
            //We also need to randomize the size of buttons a bit
            int rndSizeOfSelection = Random.Range(1, 10);
            print("==> AI rndSizeOfSelection: " + rndSizeOfSelection);
            int firstSelectedButtonRealIndex = bestStartingButton.GetComponent<ButtonManager>().GetRealIndex();
            int firstSelectedButtonColorID = bestStartingButton.GetComponent<ButtonManager>().buttonColorID;
            //<
            if (rndSizeOfSelection >= 2)
                CheckSelectableButton(firstSelectedButtonRealIndex - 1, firstSelectedButtonColorID, ref selectedButtons);
            //<<
            if (rndSizeOfSelection >= 3)
                CheckSelectableButton(firstSelectedButtonRealIndex - 2, firstSelectedButtonColorID, ref selectedButtons);
            //>
            if (rndSizeOfSelection >= 4)
                CheckSelectableButton(firstSelectedButtonRealIndex + 1, firstSelectedButtonColorID, ref selectedButtons);
            //>>
            if (rndSizeOfSelection >= 5)
                CheckSelectableButton(firstSelectedButtonRealIndex + 2, firstSelectedButtonColorID, ref selectedButtons);

            //Debug
            print("<b>Final AI decision Count (total buttons): </b>" + selectedButtons.Count);
        }

        public void CheckSelectableButton(int comparableIndex, int checkColorId, ref List<GameObject> selectedButtons)
        {
            // 인덱스 초과시 배제
            if (comparableIndex < 0 || comparableIndex >= BoardManager.instance.boardSize.x * BoardManager.instance.boardSize.y) return;

            // 서로 다른색이면 배제
            if (BoardManager.instance.GetBoardButtonColorID(comparableIndex) != checkColorId) return;

            // 이미 Finalized 되었다면 배제
            if (BoardManager.instance.IsBoardButtonFinalized(comparableIndex)) return;

            // 후보에 추가
            selectedButtons.Add(BoardManager.instance.boardButtons[comparableIndex]);
        }
    }
}