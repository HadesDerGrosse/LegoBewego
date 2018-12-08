using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorField : MonoBehaviour {

    public static VectorField instance;

    public List<Rigidbody> particles;

    public float forceFactor = 50;


    private VectorfieldFraction[,] vectorfield;
    // store the current extents of the vectorfield
    private Vector2 vectorfieldOrigin;
    private Vector2 vectorfieldOffset;

    // size of array 
    private int vfX = 0;
    private int vfY = 0;

    private float fieldSize = 1;

    // plane wo which the frustums of the camera are projected against
    private Plane ground;

    // BL, BR, TL, TR
    // screenCorners holds coordinates in screenSpace
    private Vector3[] screenCorners;
    private Vector3[] corners;
    private Bounds cornerBounds;



    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }


    // Use this for initialization
    void Start () {
        
        ground = new Plane(Vector3.up, Vector3.zero);
        corners = new Vector3[4];
        vectorfieldOffset = new Vector2(0,0);
        screenCorners = new Vector3[]{
            new Vector3(0, 0, 0),
            new Vector3(Camera.main.pixelWidth, 0, 0),
            new Vector3(0, Camera.main.pixelHeight, 0),
            new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0) };



        RaycastCameraFrustum();

        vectorfieldOrigin = new Vector2(Mathf.Floor(cornerBounds.min.x), Mathf.Floor(cornerBounds.min.z));
        Vector2 vectorfieldMax = new Vector2(Mathf.Ceil(cornerBounds.max.x), Mathf.Ceil(cornerBounds.max.z));


        // create array with the correct amount of indizes
        vfX = (int)(vectorfieldMax.x - vectorfieldOrigin.x);
        vfY = (int)(vectorfieldMax.y - vectorfieldOrigin.y);
        vectorfield = new VectorfieldFraction[vfX, vfY];

        for(int i=0; i<vfX; i++)
        {
            for (int j = 0; j < vfY; j++)
            {
                vectorfield[i, j] = new VectorfieldFraction();
            }
        }

    }



    // Update is called once per frame
    void Update () {

        RaycastCameraFrustum();

        Vector2 newOrigin = new Vector2(Mathf.Floor(cornerBounds.min.x), Mathf.Floor(cornerBounds.min.z));

        // if the origin has moved the array needs to be moved as well
        // this is done by discarding entries that are not needed anymore

        if (newOrigin.x > vectorfieldOrigin.x + vectorfieldOffset.x)
        {
            // raise Offset
            vectorfieldOffset.Set(vectorfieldOffset.x+1, vectorfieldOffset.y);

            // clear fields
            for (int i = 0; i < vfY; i++)
            {
                vectorfield[(int)(vfX + ((vectorfieldOffset.x - 1) % vfX)) % vfX, i] = new VectorfieldFraction();
            }
        }

        if (newOrigin.x < vectorfieldOrigin.x + vectorfieldOffset.x)
        {
            // reduce Offset
            vectorfieldOffset.Set(vectorfieldOffset.x - 1, vectorfieldOffset.y);

            // clear fields
            for (int i = 0; i < vectorfield.GetLength(1); i++)
            {
                vectorfield[(int)(vfX + ((vectorfieldOffset.x - 1)%vfX)) % vfX, i] = new VectorfieldFraction();
            }
        }

        if (newOrigin.y > vectorfieldOrigin.y + vectorfieldOffset.y)
        {
            // raise Offset
            vectorfieldOffset.Set(vectorfieldOffset.x, vectorfieldOffset.y+1);

            // clear fields
            for (int i = 0; i < vfX; i++)
            {
                vectorfield[i, (int)(vfY + ((vectorfieldOffset.y - 1) % vfY)) % vfY] = new VectorfieldFraction();
            }
        }

        if (newOrigin.y < vectorfieldOrigin.y + vectorfieldOffset.y)
        {
            // raise Origin
            vectorfieldOffset.Set(vectorfieldOffset.x, vectorfieldOffset.y - 1);

            // clear fields
            for (int i = 0; i < vfX; i++)
            {
                vectorfield[i, (int)(vfY + ((vectorfieldOffset.y - 1) % vfY)) % vfY] = new VectorfieldFraction();
            }
        }


        // force propagation

        /*
         * for all cells (x,y) {
      px = vx(x-1,y  ) - vx(x+1,y  );
      py = vy(x  ,y-1) - vy(x  ,y+1);
      p(x,y) = (px+py)*0.5;
   }

         * for all cells (x,y) {
      vx(x,y) += ( p(x-1,y  ) - p(x+1,y  ) )*0.5;
      vy(x,y) += ( p(x  ,y-1) - p(x  ,y+1) )*0.5;

      if ( frictionTurnedOn ) {
         vx(x,y) *= 0.99;
         vy(x,y) *= 0.99;
      }
   }
         * 
         */
        for (int i = 0; i < vfX; i++)
        {
            for (int j = 0; j < vfY; j++)
            {

                // diffuse
                float x = vectorfield[i, j].vel.x;
                float y = vectorfield[i, j].vel.y;

                if (i <= 0 || i >= vfX-1 || j <= 0 || j >= vfY-1)
                {
                    vectorfield[i, j].vel = vectorfield[i, j].vel * 0.95f;
                }
                else
                {
                    VectorfieldFraction frac = vectorfield[i, j];
                    float vx = frac.vel.x;
                    float vy = frac.vel.y;

                    float px = vectorfield[i - 1, j].vel.x - vectorfield[i + 1, j].vel.x;
                    float py = vectorfield[i, j - 1].vel.y - vectorfield[i, j + 1].vel.y;
                    frac.pressure = (px + py) * 0.5f;

                    vx += (vectorfield[i - 1, j].pressure - vectorfield[i + 1, j].pressure) * 0.5f;
                    vy += (vectorfield[i, j - 1].pressure - vectorfield[i, j + 1].pressure) * 0.5f;

                    vx *= 0.98f;
                    vy *= 0.98f;
                    frac.vel.Set(vx, vy);
                    /*

                        vectorfield[i, j] = 
                            (vectorfield[i, j] + 
                            (vectorfield[i-1, j] - vectorfield[i+1, j])*0.5f +
                            (vectorfield[i, j-1] - vectorfield[i, j+1])*0.5f
                            ) * 0.95f
                            ;
                        */
                }

            }
        }


        // add force to particles
        foreach(Rigidbody particle in particles)
        {
            particle.AddForce(forceFactor * getForce(particle.GetComponent<Transform>().position));
        }


    }


    // DEBUG
    void OnDrawGizmos()
    {
        if (Application.isPlaying && DebugManager.instance.debugVectorField)
        {
            //draw screen edge points
            for(int i=0; i< corners.Length; i++)
            {
                Gizmos.DrawSphere(corners[i], 0.5f);
                Gizmos.DrawWireCube(cornerBounds.center, cornerBounds.size);
            }

            for (int i = 0; i < vfX; i++)
            {
                for (int j = 0; j < vfY; j++)
                {
                    Vector2 shift = new Vector2(
                        Mathf.Floor((vfX+vectorfieldOffset.x-i) / vfX), 
                        Mathf.Floor((vfY+vectorfieldOffset.y-j) / vfY));

                    Vector3 start = new Vector3(
                        i + shift.x*vfX + vectorfieldOrigin.x,
                        0,
                        j + shift.y*vfY + vectorfieldOrigin.y);

                    Gizmos.DrawWireCube(start, new Vector3(1, 0.01f, 1));
                    Debug.DrawLine(start, start + Vec2ToVec3(vectorfield[i,j].vel));
                }
            }
        }
    }







    // Utility
    private void RaycastCameraFrustum()
    {
        // check how much the camera sees
        // ray cast from camera space to plane in ground axis
        for (int i = 0; i < screenCorners.Length; i++)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenCorners[i]);

            float enter = 0f;

            if (ground.Raycast(ray, out enter))
            {
                corners[i] = ray.GetPoint(enter);
            }
        }

        // create a bounding box from the camera frustums
        cornerBounds = new Bounds(corners[0], Vector3.zero);
        for (int i = 1; i < corners.Length; i++)
        {
            cornerBounds.Encapsulate(corners[i]);
        }
    }



    private int[] WorldPositionToArrayIndex(Vector3 position)
    {
        Vector2 fieldPosition = Vec3ToVec2(position) - vectorfieldOrigin;
        int x = (int)(Mathf.Round(fieldPosition.x) % vfX);
        int y = (int)(Mathf.Round(fieldPosition.y) % vfY);
        if (x < 0 || x > vfX || y < 0 || y > vfY) throw new System.IndexOutOfRangeException();
        return new int[] {Mathf.Clamp(x, 0, vfX), Mathf.Clamp(y, 0, vfY)};
    }



    private Vector2 Vec3ToVec2(Vector3 input)
    {
        return new Vector2(input.x, input.z);
    }



    private Vector3 Vec2ToVec3(Vector2 input)
    {
        return new Vector3(input.x, 0, input.y);
    }









    // IO

    public Vector3 getForce(Vector3 position)
    {
        int[] indizes;
        try
        {
            indizes = WorldPositionToArrayIndex(position);
        }
        catch (System.IndexOutOfRangeException)
        {
            return new Vector3();
        }
        int x = indizes[0];
        int y = indizes[1];
        return Vec2ToVec3(vectorfield[x,y].vel);
    }


    // input pixel coordinates, raycast to ground and add force
    public void addForce(Vector2 start, Vector2 end, int size = 0)
    {
        Ray startRay = Camera.main.ScreenPointToRay(start);
        Ray endRay = Camera.main.ScreenPointToRay(end);

        float startEnter = 0f;
        float endEnter = 0f;

        if (ground.Raycast(startRay, out startEnter) && ground.Raycast(endRay, out endEnter))
        {
            Vector3 startPos = startRay.GetPoint(startEnter);
            Vector3 endPos = endRay.GetPoint(endEnter);
            try
            {
                int[] index = WorldPositionToArrayIndex(startPos);
                Vector2 force = Vec3ToVec2(endPos - startPos);
                vectorfield[index[0], index[1]].vel = force;
            }
            catch (System.IndexOutOfRangeException)
            {
                print("Index out of range in addForce of Vector Field");
            }

        }
    }



    public void addParticle(Rigidbody rb)
    {
        particles.Add(rb);
    }

    public void removeParticle(Rigidbody rb)
    {
        particles.Remove(rb);
    }

    public void clearParticles()
    {
        particles.Clear();
    }

}



public class VectorfieldFraction
{
    public float pressure;
    public Vector2 vel;

    public VectorfieldFraction()
    {
        pressure = 0f;
        vel = new Vector2();
    }
}