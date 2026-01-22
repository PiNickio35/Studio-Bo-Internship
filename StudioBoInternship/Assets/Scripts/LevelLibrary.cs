using System;
using System.Collections.Generic;
using Base_Classes;
using UnityEngine;

public class LevelLibrary : MonoBehaviour
{
    public static LevelLibrary Instance;
    public Dictionary<int, float> sonoHp = new Dictionary<int, float>()
    {
        {1, 40}, {2, 60}, {3, 80}, {4, 100}, {5, 125}, {6, 150}, {7, 175}, {8,200}, {9, 210}, {10, 220}, {11, 230},
        {12, 240}, {13, 250}, {14, 260}, {15, 270}, {16, 280}, {17, 290}, {18, 300}, {19, 300}, {20, 300}
    };
    public Dictionary<int, float> sonoMp = new Dictionary<int, float>()
    {
        {1, 5}, {2, 10}, {3, 10}, {4, 15}, {5, 15}, {6, 20}, {7, 20}, {8,20}, {9, 20}, {10, 25}, {11, 25},
        {12, 25}, {13, 25}, {14, 25}, {15, 25}, {16, 25}, {17, 25}, {18, 25}, {19, 25}, {20, 25}
    };
    public Dictionary<int, float> sonoDefence = new Dictionary<int, float>()
    {
        {1, 4}, {2, 5}, {3, 6}, {4, 7}, {5, 8}, {6, 9}, {7, 10}, {8,10}, {9, 11}, {10, 11}, {11, 12},
        {12, 12}, {13, 13}, {14, 14}, {15, 15}, {16, 16}, {17, 17}, {18, 18}, {19, 19}, {20, 20}
    };
    public Dictionary<int, BaseAttack> sonoAttacks = new Dictionary<int, BaseAttack>();
    public Dictionary<int, BaseAttack> sonoMagic = new();
    public Dictionary<int, float> sonoStrength = new Dictionary<int, float>()
    {
        {1, 4}, {2, 4}, {3, 5}, {4, 6}, {5, 7}, {6, 8}, {7, 9}, {8, 10}, {9, 10}, {10, 11}, {11, 11},
        {12, 12}, {13, 12}, {14, 13}, {15, 13}, {16, 14}, {17, 14}, {18, 15}, {19, 15}, {20, 16}
    };
    public Dictionary<int, float> sonoAgility = new Dictionary<int, float>()
    {
        {1, 6}, {2, 8}, {3, 10}, {4, 12}, {5, 14}, {6, 16}, {7, 18}, {8,20}, {9, 22}, {10, 24}, {11, 26},
        {12, 28}, {13, 30}, {14, 32}, {15, 34}, {16, 36}, {17, 37}, {18, 38}, {19, 39}, {20, 40}
    };
    public Dictionary<int, float> sonoWisdom = new Dictionary<int, float>()
    {
        {1, 2}, {2, 2}, {3, 3}, {4, 3}, {5, 4}, {6, 4}, {7, 5}, {8,5}, {9, 5}, {10, 6}, {11, 6},
        {12, 6}, {13, 6}, {14, 7}, {15, 7}, {16, 7}, {17, 7}, {18, 8}, {19, 8}, {20, 8}
    };
    public Dictionary<int, float> mayHp = new Dictionary<int, float>()
    {
        {1, 25}, {2, 30}, {3, 35}, {4, 40}, {5, 50}, {6, 60}, {7, 70}, {8,80}, {9, 90}, {10, 100}, {11, 105},
        {12, 105}, {13, 105}, {14, 110}, {15, 110}, {16, 110}, {17, 115}, {18, 115}, {19, 120}, {20, 120}
    };
    public Dictionary<int, float> mayMp = new Dictionary<int, float>()
    {
        {1, 10}, {2, 15}, {3, 20}, {4, 25}, {5, 25}, {6, 25}, {7, 30}, {8,30}, {9, 30}, {10, 30}, {11, 35},
        {12, 35}, {13, 35}, {14, 40}, {15, 40}, {16, 45}, {17, 45}, {18, 50}, {19, 50}, {20, 50}
    };
    public Dictionary<int, float> mayDefence = new Dictionary<int, float>()
    {
        {1, 2}, {2, 2}, {3, 2}, {4, 3}, {5, 3}, {6, 4}, {7, 4}, {8,4}, {9, 5}, {10, 5}, {11, 6},
        {12, 6}, {13, 7}, {14, 7}, {15, 8}, {16, 8}, {17, 9}, {18, 9}, {19, 10}, {20, 10}
    };
    public Dictionary<int, BaseAttack> mayAttacks = new Dictionary<int, BaseAttack>();
    public Dictionary<int, BaseAttack> mayMagic = new();
    public Dictionary<int, float> mayStrength = new Dictionary<int, float>()
    {
        {1, 2}, {2, 2}, {3, 2}, {4, 2}, {5, 3}, {6, 3}, {7, 3}, {8,3}, {9, 3}, {10, 4}, {11, 4},
        {12, 4}, {13, 4}, {14, 4}, {15, 4}, {16, 5}, {17, 5}, {18, 5}, {19, 5}, {20, 5}
    };
    public Dictionary<int, float> mayAgility = new Dictionary<int, float>()
    {
        {1, 4}, {2, 4}, {3, 5}, {4, 6}, {5, 7}, {6, 8}, {7, 9}, {8,10}, {9, 11}, {10, 11}, {11, 12},
        {12, 12}, {13, 13}, {14, 13}, {15, 14}, {16, 14}, {17, 15}, {18, 15}, {19, 16}, {20, 16}
    };
    public Dictionary<int, float> mayWisdom = new Dictionary<int, float>()
    {
        {1, 8}, {2, 10}, {3, 12}, {4, 13}, {5, 14}, {6, 15}, {7, 16}, {8,16}, {9, 17}, {10, 18}, {11, 18},
        {12, 19}, {13, 20}, {14, 20}, {15, 20}, {16, 22}, {17, 22}, {18, 22}, {19, 24}, {20, 24}
    };
    public Dictionary<int, float> andaniHp = new Dictionary<int, float>()
    {
        {1, 70}, {2, 100}, {3, 125}, {4, 150}, {5, 175}, {6, 200}, {7, 225}, {8,250}, {9, 275}, {10, 300}, {11, 320},
        {12, 340}, {13, 360}, {14, 380}, {15, 400}, {16, 420}, {17, 440}, {18, 460}, {19, 480}, {20, 500}
    };
    public Dictionary<int, float> andaniMp = new Dictionary<int, float>()
    {
        {1, 0}, {2, 0}, {3, 0}, {4, 0}, {5, 0}, {6, 0}, {7, 0}, {8, 0}, {9, 0}, {10, 0}, {11, 0},
        {12, 0}, {13, 0}, {14, 0}, {15, 0}, {16, 0}, {17, 0}, {18, 0}, {19, 0}, {20, 0}
    };
    public Dictionary<int, float> andaniDefence = new Dictionary<int, float>()
    {
        {1, 6}, {2, 8}, {3, 10}, {4, 12}, {5, 14}, {6, 16}, {7, 18}, {8,20}, {9, 22}, {10, 24}, {11, 26},
        {12, 28}, {13, 30}, {14, 32}, {15, 34}, {16, 36}, {17, 37}, {18, 38}, {19, 39}, {20, 40}
    };
    public Dictionary<int, BaseAttack> andaniAttacks = new Dictionary<int, BaseAttack>();
    public Dictionary<int, float> andaniStrength = new Dictionary<int, float>()
    {
        {1, 6}, {2, 7}, {3, 8}, {4, 9}, {5, 9}, {6, 10}, {7, 10}, {8,11}, {9, 11}, {10, 12}, {11, 12},
        {12, 13}, {13, 13}, {14, 14}, {15, 14}, {16, 15}, {17, 15}, {18, 16}, {19, 18}, {20, 20}
    };
    public Dictionary<int, float> andaniAgility = new Dictionary<int, float>()
    {
        {1, 2}, {2, 2}, {3, 2}, {4, 2}, {5, 3}, {6, 3}, {7, 3}, {8,4}, {9, 4}, {10, 4}, {11, 5},
        {12, 5}, {13, 5}, {14, 5}, {15, 6}, {16, 6}, {17, 6}, {18, 7}, {19, 7}, {20, 8}
    };
    public Dictionary<int, float> andaniWisdom = new Dictionary<int, float>()
    {
        {1, 0}, {2, 0}, {3, 0}, {4, 0}, {5, 0}, {6, 0}, {7, 0}, {8,0}, {9, 0}, {10, 0}, {11, 0},
        {12, 0}, {13, 0}, {14, 0}, {15, 0}, {16, 0}, {17, 0}, {18, 0}, {19, 0}, {20, 100}
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
