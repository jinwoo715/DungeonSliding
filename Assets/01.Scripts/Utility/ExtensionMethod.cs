using UnityEngine;

public class ExtensionMethod : MonoBehaviour
{
    public static Quaternion RandomFourDirection()
    {
        System.Random rand = new System.Random();

        // 0 왼쪽
        // 1 위
        // 2 오른쪽
        // 3 아래쪽
        int rndDir = rand.Next(0, 4);
        float y = 0;

        switch (rndDir)
        {
            case 0:
                y = 270;
                break;
            case 1:
                y = 0;
                break;
            case 2:
                y = 90;
                break;
            case 3:
                y = 180;
                break;
        }

        return Quaternion.Euler(0,y,0);
    }
}
