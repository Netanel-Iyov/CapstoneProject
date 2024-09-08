using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapModel
{

    private static MapModel _instance;
    public static MapModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MapModel();
            }
            return _instance;
        }
    }

    private MapModel()
    {

    }

    public Sprite GetImageAsSprite(string imagePath)
    {
        return Resources.Load<Sprite>(imagePath);
    }


}
