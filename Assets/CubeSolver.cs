using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class RubiksCubeSolver
{
    internal static class TransitionModel
    {
        private static readonly IDictionary<string, int> MOVE_INT_MAP = new Dictionary<string, int>{
                {"U", 0}, {"U2", 1}, {"U'", 2},
                {"D", 3}, {"D2", 4}, {"D'", 5},
                {"F", 6}, {"F2", 7}, {"F'", 8},
                {"B", 9}, {"B2", 10}, {"B'", 11},
                {"L", 12}, {"L2", 13}, {"L'", 14},
                {"R", 15}, {"R2", 16}, {"R'", 17},
            };

        private static readonly int[,] CUBIES_MAP = {
                {  0,  1,  2,  3,  0,  1,  2,  3 },   // U
                {  4,  7,  6,  5,  4,  5,  6,  7 },   // D
                {  0,  9,  4,  8,  0,  3,  5,  4 },   // F
                {  2, 10,  6, 11,  2,  1,  7,  6 },   // B
                {  3, 11,  7,  9,  3,  2,  6,  5 },   // L
                {  1,  8,  5, 10,  1,  0,  4,  7 },   // R
            };

        public static int MoveToInt(string move)
        {
            return MOVE_INT_MAP[move];
        }

        public static string MoveToString(int move)
        {
            return MOVE_INT_MAP.FirstOrDefault(x => x.Value == move).Key;
        }

        public static int GetInverseMove(int move)
        {
            return move + 2 - 2 * (move % 3);
        }

        public static string GetInverseMove(string move)
        {
            int moveInt = MoveToInt(move);
            int revMove = moveInt + 2 - 2 * (moveInt % 3);

            return MoveToString(revMove);
        }

        public static string[] ReverseMoveList(string[] moves)
        {
            string[] reveredMoves = (string[]) moves.Clone();

            for (int i = 0; i < reveredMoves.Length; i++)
            {
                reveredMoves[i] = GetInverseMove(reveredMoves[i]);
            }
            Array.Reverse(reveredMoves);

            return reveredMoves;
        }

        public static int[] ApplyMove(int move, int[] state)
        {
            int rotations = move % 3 + 1;
            int face = move / 3;

            // dont alter original state
            state = (int[])state.Clone();

            while (rotations-- > 0)
            {
                // take a copy of current state
                int[] oldState = (int[])state.Clone();

                // loop over 8 cubies in face
                for (int i = 0; i < 8; i++)
                {
                    // check if the current cubie is a corner cubie or not
                    // cubies from 0 to 3 are edge cubies, 4 to 7 are corner cubies
                    int isCorner = Convert.ToInt32(i > 3);

                    // get the target cubie POSITION in state
                    // using index
                    //
                    // if cubie is a corner cubie, we add 12 to the retrieved cubie position value
                    // the difference between any edge cubie and its adjacent corner cubie is 12
                    // edge cubies start at position 0, there are 11 of them
                    // corner cubies start at position 12
                    int targetCubiePosition = CUBIES_MAP[face, i] + isCorner * 12;

                    // get position of cubie to replace target cubie
                    // replacement cubie of a cubie is the one in the next position so we add 1 to the index
                    // when i == 7 (last cubie) we roll back because there are no next cubies
                    int replacementCubiePosition = CUBIES_MAP[face, i == 7 ? 0 : i + 1] + isCorner * 12;

                    // calculate orientation delta 
                    int orientationDelta = 0;

                    // if current cubie is an edge cubie and current face is either front or back
                    // set orientation delta is 1
                    // otherwise set it to 0
                    if (i < 4)
                    {
                        orientationDelta = Convert.ToInt32(face > 1 && face < 4);
                    }
                    // if current cubie is a corner cubie and current face is either up or down
                    // set orientation delta to 0
                    // otherwise if current face is left or right
                    // set it to 2 for upper left and lower left corners (represented by even numbers)
                    // and set it to 1 for upper right and lower right (represented by even numbers)
                    //            & 1 of an even number is 0
                    else
                    {
                        orientationDelta = (face < 2) ? 0 : 2 - (i & 1);
                    }

                    // preform the replacement
                    state[targetCubiePosition] = oldState[replacementCubiePosition];

                    // apply the orientation delta
                    state[targetCubiePosition + 20] = oldState[replacementCubiePosition + 20] + orientationDelta;
                    if (rotations == 0)
                        state[targetCubiePosition + 20] %= 2 + isCorner;
                }
            }
            return state;
        }

    }

    internal static class StateConverter
    {
        // converts a state string to a state array
        // UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB
        /*
                        |************|
                        |*U1**U2**U3*|
                        |************|
                        |*U4**U5**U6*|
                        |************|
                        |*U7**U8**U9*|
            ************|************|************|************|
            *L1**L2**L3*|*F1**F2**F3*|*R1**R2**F3*|*B1**B2**B3*|
            ************|************|************|************|
            *L4**L5**L6*|*F4**F5**F6*|*R4**R5**R6*|*B4**B5**B6*|
            ************|************|************|************|
            *L7**L8**L9*|*F7**F8**F9*|*R7**R8**R9*|*B7**B8**B9*|
            ************|************|************|************|
                        |*D1**D2**D3*|
                        |************|
                        |*D4**D5**D6*|
                        |************|
                        |*D7**D8**D9*|
                        |************|
         */

        // UF UR UB UL DF DR DB DL FR FL BR BL
        // 0  1  2  3  4  5  6  7  8  9  10 11
        //
        // UFR URB UBL ULF DRF DFL DLB DBR
        // 12  13  14  15  16  17  18  19 
        private static readonly IDictionary<string, int[]> CubieIntMap = new Dictionary<string, int[]>{
                {"UF", new int[] {0,0} }, {"FU", new int[] {0,1} },
                {"UR", new int[] {1,0} }, {"RU", new int[] {1,1} },
                {"UB", new int[] {2,0} }, {"BU", new int[] {2,1} },
                {"UL", new int[] {3,0} }, {"LU", new int[] {3,1} },
                {"DF", new int[] {4,0} }, {"FD", new int[] {4,1} },
                {"DR", new int[] {5,0} }, {"RD", new int[] {5,1} },
                {"DB", new int[] {6,0} }, {"BD", new int[] {6,1} },
                {"DL", new int[] {7,0} }, {"LD", new int[] {7,1} },
                {"FR", new int[] {8,0} }, {"RF", new int[] {8,1} },
                {"FL", new int[] {9,0} }, {"LF", new int[] {9,1} },
                {"BR", new int[] {10,0} }, {"RB", new int[] {10,1} },
                {"BL", new int[] {11,0} }, {"LB", new int[] {11,1} },
                {"UFR", new int[] {12,0} },{"URF", new int[] {12,2} },{"FUR", new int[] {12,1} },{"RFU", new int[] {12,2} },{"FRU", new int[] {12,2} },{"RUF", new int[] {12,1} },
                {"URB", new int[] {13,0} },{"UBR", new int[] {13,2} },{"RUB", new int[] {13,1} },{"BRU", new int[] {13,2} },{"RBU", new int[] {13,2} },{"BUR", new int[] {13,1} },
                {"UBL", new int[] {14,0} },{"ULB", new int[] {14,2} },{"BUL", new int[] {14,1} },{"LBU", new int[] {14,2} },{"BLU", new int[] {14,2} },{"LUB", new int[] {14,1} },
                {"ULF", new int[] {15,0} },{"UFL", new int[] {15,2} },{"LUF", new int[] {15,1} },{"FLU", new int[] {15,2} },{"LFU", new int[] {15,2} },{"FUL", new int[] {15,1} },
                {"DRF", new int[] {16,0} },{"DFR", new int[] {16,2} },{"RDF", new int[] {16,1} },{"FRD", new int[] {16,2} },{"RFD", new int[] {16,2} },{"FDR", new int[] {16,1} },
                {"DFL", new int[] {17,0} },{"DLF", new int[] {17,2} },{"FDL", new int[] {17,1} },{"LFD", new int[] {17,2} },{"FLD", new int[] {17,2} },{"LDF", new int[] {17,1} },
                {"DLB", new int[] {18,0} },{"DBL", new int[] {18,2} },{"LDB", new int[] {18,1} },{"BLD", new int[] {18,2} },{"LBD", new int[] {18,2} },{"BDL", new int[] {18,1} },
                {"DBR", new int[] {19,0} },{"DRB", new int[] {19,2} },{"BDR", new int[] {19,1} },{"RBD", new int[] {19,2} },{"BRD", new int[] {19,2} },{"RDB", new int[] {19,1} },
            };

        private static readonly int[][] positionIndices = new int[][]
        {
                new int[]{07,19},          // p00 UF
                new int[]{05,10},          // p01 UR
                new int[]{01,46},          // p02 UB
                new int[]{03,37},          // p03 UL
                new int[]{28,25},          // p04 DF
                new int[]{32,16},          // p05 DR
                new int[]{34,52},          // p06 DB
                new int[]{30,43},          // p07 DL
                new int[]{23,12},          // p08 FR
                new int[]{21,41},          // p09 FL
                new int[]{48,14},          // p10 BR
                new int[]{50,39},          // p11 BL
                new int[]{08,20,09},       // p12 UFR
                new int[]{02,11,45},       // p13 URB 
                new int[]{00,47,36},       // p14 UBL
                new int[]{06,38,18},       // p15 ULF
                new int[]{29,15,26},       // p16 DRF
                new int[]{27,24,44},       // p17 DFL
                new int[]{33,42,53},       // p18 DLB
                new int[]{35,51,17},       // p19 DBR
        };

        public static int[] Convert(string state)
        {
            int[] convertedState = Enumerable.Repeat(0, 40).ToArray();
            string[] positionValues = Enumerable.Repeat("", 20).ToArray();

            for (int i = 0; i < 20; i++)
            {
                string positionValue = "";
                int[] positionIdx = positionIndices[i];
                for (int j = 0; j < positionIdx.Length; j++)
                {
                    positionValue += state[positionIdx[j]];
                }
                positionValues[i] = positionValue;
            }

            for (int i = 0; i < 20; i++)
            {
                convertedState[i] = CubieIntMap[positionValues[i]][0];
                convertedState[i + 20] = CubieIntMap[positionValues[i]][1];
            }

            return convertedState;
        }
    }

    internal static class Thistlethwaite
    {
        // A variant of Thistlethwaite algorithm written by Stephan Pochmann for this cube contest
        // https://tomas.rokicki.com/cubecontest/
        // https://tomas.rokicki.com/cubecontest/winners.html
        // https://tomas.rokicki.com/cubecontest/pochmann.zip
        //
        public static readonly int[][] PHASE_ALLOWED_MOVES = new int[][]
        {
                // 0
                new int[]{},

                // 1: U U2 U' D D2 D' F F2 F' B B2 B' L L2 L' R R2 R'
                new int[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, },

                // 2: U U2 U' D D2 D' F2 B2 L L2 L' R R2 R'
                new int[]{ 0, 1, 2, 3, 4, 5, 7, 10, 12, 13, 14, 15, 16, 17 }, 

                // 3: U U2 U' D D2 D' F2 B2 L2 R2
                new int[]{ 0, 1, 2, 3, 4, 5, 7, 10, 13, 16 },

                // 4: U2 D2 F2 B2 L2 R2
                new int[]{ 1, 4, 7, 10, 13, 16 }
        };

        public static int[] GetStateHash(int[] state, int phase)
        {
            int[] StateHash = new int[] { };

            // phase 1: Edge orientations
            // copy indices 20-31 of state array 
            // representing edge orientations
            if (phase == 1)
            {
                StateHash = new int[12];
                Array.Copy(state, 20, StateHash, 0, 12);
            }

            // phase 2: Corner orientations, E slice edges
            if (phase == 2)
            {
                StateHash = new int[8];
                Array.Copy(state, 31, StateHash, 0, 8);
                for (int e = 0; e < 12; e++)
                    StateHash[0] |= (state[e] / 8) << e;
            }

            // phase 3: Edge slices M and S, corner tetrads, overall parity
            if (phase == 3)
            {
                StateHash = new int[3] { 0, 0, 0 };
                for (int e = 0; e < 12; e++)
                    StateHash[0] |= ((state[e] > 7) ? 2 : (state[e] & 1)) << (2 * e);
                for (int c = 0; c < 8; c++)
                    StateHash[1] |= ((state[c + 12] - 12) & 5) << (3 * c);
                for (int i = 12; i < 20; i++)
                    for (int j = i + 1; j < 20; j++)
                        StateHash[2] ^= Convert.ToInt32(state[i] > state[j]);
            }

            // phase 4: Whole cube
            if (phase == 4)
            {
                StateHash = state;
            }

            return StateHash;
        }

        public class StateHashComparer : IEqualityComparer<int[]>
        {
            public bool Equals(int[] x, int[] y)
            {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(int[] obj)
            {
                int result = 17;
                for (int i = 0; i < obj.Length; i++)
                {
                    unchecked { result = result * 23 + obj[i]; }
                }
                return result;
            }
        }
    }

    internal static class Search
    {
        private enum DIRECTION
        {
            UNDEFINED,
            FORWARD,
            BACKWARD
        };

        private class Node
        {
            public int[] state;
            public int[] stateHash;
            public int[] predecessorHash;
            public int previousMove;
            public Search.DIRECTION direction;
            public Node(int[] state, int phase, DIRECTION direction, int previousMove, int[] predecessorHash)
            {
                this.state           = state;
                this.stateHash       = Thistlethwaite.GetStateHash(this.state, phase);
                this.direction       = direction;
                this.previousMove    = previousMove;
                this.predecessorHash = predecessorHash;
            }
        }

        public static LinkedList<int> BidirectionalBFS(int[] startState, int[] goalState, int phase)
        {

            // create start and goal nodes
            Node startNode = new Node(startState, phase, DIRECTION.FORWARD, -1, null);
            Node goalNode  = new Node(goalState, phase, DIRECTION.BACKWARD, -1, null);

            // initialize fringe and lookup table

            // create fringe
            Queue<Node> fringe = new Queue<Node>();

            // enqueue start state and goal
            fringe.Enqueue(startNode);
            fringe.Enqueue(goalNode);

            // create lookup table
            Dictionary<int[], Node> visited = new Dictionary<int[], Node>(new Thistlethwaite.StateHashComparer());

            // mark start and goal as visited
            visited[startNode.stateHash] = startNode;
            visited[goalNode.stateHash]  = goalNode;

            while (true)
            {
                // Get next state from fringe
                Node currentNode = fringe.Dequeue();

                // get allowed moves for the phase we're in
                int[] currentPhaseAllowedMoves = Thistlethwaite.PHASE_ALLOWED_MOVES[phase];

                // apply allowed moves to current state and get successor states
                for (int i = 0; i < currentPhaseAllowedMoves.Length; i++)
                {
                    int move = currentPhaseAllowedMoves[i];

                    // generate a successor
                    Node newNode = new Node(
                        TransitionModel.ApplyMove(move, currentNode.state),
                        phase,
                        DIRECTION.UNDEFINED,
                        -1,
                        null);

                    // check if successor was visited 
                    Node tmpNode = null;
                    visited.TryGetValue(newNode.stateHash, out tmpNode);

                    // if it was visited from the same direction
                    // skip it
                    if (tmpNode != null && tmpNode.direction == currentNode.direction) continue;

                    // if it was visited from the opposite direction
                    // then we have found a meeting point
                    // terminate the search and construct the full path
                    if (tmpNode != null && tmpNode.direction != currentNode.direction)
                    {
                        newNode = tmpNode;

                        // if current state direction is backward, swap nodes and inverse current move
                        if (currentNode.direction == DIRECTION.BACKWARD)
                        {
                            Node pivot  = newNode;
                            newNode     = currentNode;
                            currentNode = pivot;

                            move = TransitionModel.GetInverseMove(move);
                        }

                        LinkedList<int> moves = new LinkedList<int>();
                        moves.AddFirst(move);

                        // traverse backward to beginning state
                        while (!currentNode.stateHash.SequenceEqual(startNode.stateHash))
                        {
                            moves.AddFirst(currentNode.previousMove);
                            currentNode = visited[currentNode.predecessorHash];
                        }

                        // traverse forward to goal state
                        while (!newNode.stateHash.SequenceEqual(goalNode.stateHash))
                        {
                            moves.AddLast(TransitionModel.GetInverseMove(newNode.previousMove));
                            newNode = visited[newNode.predecessorHash];
                        }

                        return moves;
                    }

                    // add new state to fringe and mark it as visited
                    if (newNode.direction == DIRECTION.UNDEFINED)
                    {
                        newNode.direction       = currentNode.direction;
                        newNode.previousMove    = move;
                        newNode.predecessorHash = currentNode.stateHash;

                        fringe.Enqueue(newNode);
                        visited[newNode.stateHash] = newNode;
                    }
                }
            }
        }
    }

    // UF UR UB UL DF DR DB DL FR FL BR BL
    // 0  1  2  3  4  5  6  7  8  9  10 11
    //
    // UFR URB UBL ULF DRF DFL DLB DBR
    // 12  13  14  15  16  17  18  19 

    // UF UR UB UL DF DR DB DL FR FL BR BL
    // 20 21 22 23 24 25 26 27 28 29 30 31
    //
    // UFR URB UBL ULF DRF DFL DLB DBR
    // 32  33  34  35  36  37  38  39

    // state array is read as follows:
    //   index represents cubie position on map
    //   value represents a cubie numeric code
    //   follow the table above
    //   

    public static readonly int[] DEFAULT_GOAL_STATE = {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

    private int[] currentState;
    private int[] goalState;


    // initializers
    public void initialize(int[] initialState, int[] goalState = null)
    {
        this.currentState = initialState;
        this.goalState = goalState == null ? DEFAULT_GOAL_STATE : goalState;
    }
    public void initialize(string initialState, int[] goalState = null)
    {
        this.currentState = StateConverter.Convert(initialState);
        this.goalState = goalState == null ? DEFAULT_GOAL_STATE : goalState;
    }

    public void initialize(int[] initialState, string goalState)
    {
        this.currentState = initialState;
        this.goalState = StateConverter.Convert(goalState);
    }

    public void initialize(string initialState, string goalState)
    {
        this.currentState = StateConverter.Convert(initialState);
        this.goalState = StateConverter.Convert(goalState);
    }
    public void initialize(string[] initialMoves)
    {
        this.currentState = (int[])DEFAULT_GOAL_STATE.Clone();
        foreach (string move in initialMoves)
        {
            this.currentState = TransitionModel.ApplyMove(TransitionModel.MoveToInt(move), this.currentState);
        }
        this.goalState = DEFAULT_GOAL_STATE;
    }

    public string[] Solve()
    {
        int phase = 0;
        List<String> moves = new List<String>();

        while (++phase < 5)
        {
            int[] currentStateHash = Thistlethwaite.GetStateHash(this.currentState, phase);
            int[] goalStateHash = Thistlethwaite.GetStateHash(this.goalState, phase);

            // if we already at goal 
            if (currentStateHash.SequenceEqual(goalStateHash))
                continue;

            LinkedList<int> phaseMoves = Search.BidirectionalBFS(this.currentState, this.goalState, phase);

            // apply phase solution to cube state
            foreach (int move in phaseMoves)
            {
                // apply move 
                this.currentState = TransitionModel.ApplyMove(move, this.currentState);

                String moveString = TransitionModel.MoveToString(move);
                moves.Add(moveString);
            }
        }

        return moves.ToArray();
    }

}


public class CubeSolver : MonoBehaviour
{

    private CubeState cubeState;
    private ReadCube readCube;
    private AutomaticMovement automaticMovement;
    // Start is called before the first frame update
    void Start()
    {
        cubeState = FindObjectOfType<CubeState>();
        readCube = FindObjectOfType<ReadCube>();
        automaticMovement = FindObjectOfType<AutomaticMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SolveCube()
    {
        if (CubeState.autoRotating) return;

        readCube.ReadState();
        string stateString = cubeState.GetStateString();
        RubiksCubeSolver solver = new RubiksCubeSolver();
        solver.initialize(stateString);
        string[] solution = solver.Solve();
        AutomaticMovement.moveList = solution.ToList();
    }

    public void SolveWithCustomState(string customState)
    {
        if (CubeState.autoRotating) return;

        readCube.ReadState();
        string stateString = cubeState.GetStateString();

        RubiksCubeSolver solverLeft  = new RubiksCubeSolver();
        RubiksCubeSolver solverRight = new RubiksCubeSolver();

        // Go to solved state
        solverLeft.initialize(stateString);
        string[] solutionLeft = solverLeft.Solve();


        // Go to custom state
        solverRight.initialize(customState);
        string[] solutionRight = RubiksCubeSolver.TransitionModel.ReverseMoveList(solverRight.Solve());

        string[] solution = new string[solutionLeft.Length + solutionRight.Length];
        solutionLeft.CopyTo(solution, 0);
        solutionRight.CopyTo(solution, solutionLeft.Length);

        AutomaticMovement.moveList = solution.ToList();
    }
}
