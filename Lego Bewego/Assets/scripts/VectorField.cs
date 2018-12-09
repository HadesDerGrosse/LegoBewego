using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorField : MonoBehaviour {

    public static VectorField instance;

    public List<Rigidbody> particles;

    public float forceFactor = 10;

    public float heroAttractionForce = 10;
    public float heroAttractionDist = 30;

    public float maxForce = 15;

    public bool visualizeVectors = true;

    public Material lineMat;

    private VectorfieldFraction[,] vectorfield;
    // store the current extents of the vectorfield
    private Vector2 vectorfieldOrigin;
    private Vector2 vectorfieldOffset;
    private int grow = 3;

    // size of array 
    private int vfX = 0;
    private int vfY = 0;

    // plane wo which the frustums of the camera are projected against
    private Plane ground;

    // BL, BR, TL, TR
    // screenCorners holds coordinates in screenSpace
    private Vector3[] screenCorners;
    private Vector3[] corners;
    private Bounds cornerBounds;

    private HeroStone hero;

    private LineRenderer lineRenderer;



    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }


    // Use this for initialization
    void Start () {

        hero = HeroStone.getInstance();

        lineRenderer = GetComponent<LineRenderer>();

        ground = new Plane(Vector3.up, Vector3.zero);
        corners = new Vector3[4];
        vectorfieldOffset = new Vector2(0,0);
        screenCorners = new Vector3[]{
            new Vector3(0, 0, 0),
            new Vector3(Camera.main.pixelWidth, 0, 0),
            new Vector3(0, Camera.main.pixelHeight, 0),
            new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0) };



        RaycastCameraFrustum();

        vectorfieldOrigin = new Vector2(Mathf.Floor(cornerBounds.min.x - grow), Mathf.Floor(cornerBounds.min.z - grow));
        Vector2 vectorfieldMax = new Vector2(Mathf.Ceil(cornerBounds.max.x + grow), Mathf.Ceil(cornerBounds.max.z + grow));


        // create array with the correct amount of indizes
        vfX = (int)(vectorfieldMax.x - vectorfieldOrigin.x);
        vfY = (int)(vectorfieldMax.y - vectorfieldOrigin.y);
        vectorfield = new VectorfieldFraction[vfX, vfY];
        resetVectorfield();

    }



    // Update is called once per frame
    void Update () {

        hero = HeroStone.getInstance();

        RaycastCameraFrustum();

        Vector2 newOrigin = new Vector2(Mathf.Floor(cornerBounds.min.x - grow), Mathf.Floor(cornerBounds.min.z - grow));

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
        // buffer
        for (int i = 0; i < vfX; i++)
        {
            for (int j = 0; j < vfY; j++)
            {
                vectorfield[i,j].velBuffer = vectorfield[i, j].vel;
            }
        }
        for (int i = 0; i < vfX; i++)
        {
            for (int j = 0; j < vfY; j++)
            {
                // diffuse
                vectorfield[i, j].vel = Diffuse(i, j);
            }
        }


        // add force to particles
        foreach(Rigidbody particle in particles)
        {
            particle.AddForce(forceFactor * getForce(particle.GetComponent<Transform>().position));
            if (particle.gameObject != hero)
            {
                Vector3 forceVector = (hero.transform.position - particle.transform.position);

                if (forceVector.magnitude < heroAttractionDist)
                {
                    particle.GetComponent<Rigidbody>().AddForce(
                        forceVector.normalized * 
                        heroAttractionForce
                        );
                    particle.GetComponent<Rigidbody>().AddForce( 
                       - forceVector.normalized * 
                       (heroAttractionForce * 2f * (Mathf.Lerp(1,0,forceVector.magnitude/heroAttractionDist)))
                       );
                }
            }
        }


    }


    // DEBUG
    void OnDrawGizmos()
    {
        //DrawLines();
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



    public void DrawLines()
    {
        if (visualizeVectors)
        {
            for (int i = 0; i < vfX; i++)
            {
                for (int j = 0; j < vfY; j++)
                {
                    if (vectorfield[i, j].vel.magnitude > 1)
                    {
                        Vector2 shift = new Vector2(
                                Mathf.Floor((vfX + vectorfieldOffset.x - i) / vfX),
                                Mathf.Floor((vfY + vectorfieldOffset.y - j) / vfY));

                        Vector3 start = new Vector3(
                            i + shift.x * vfX + vectorfieldOrigin.x,
                            0,
                            j + shift.y * vfY + vectorfieldOrigin.y);

                        Vector3 end = start + Vec2ToVec3(vectorfield[i, j].vel);

                        GL.Begin(GL.LINES);
                        lineMat.SetPass(0);
                        GL.Color(new Color(lineMat.color.r, lineMat.color.g, lineMat.color.b, lineMat.color.a));
                        GL.Vertex3(start.x, start.y, start.z);
                        GL.Vertex3(end.x, end.y, end.z);
                        GL.End();
                    }
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
        if (position.x < cornerBounds.min.x 
            || position.x > cornerBounds.max.x
            || position.z < cornerBounds.min.z
            || position.z > cornerBounds.max.z)
        {
            throw new System.IndexOutOfRangeException("x: " + position.x + "  z: " + position.z);
        }

        Vector2 fieldPosition = Vec3ToVec2(position) - vectorfieldOrigin;

        int x = (int)(vfX + (Mathf.Round(fieldPosition.x) % vfX)) % vfX;
        int y = (int)(vfY + (Mathf.Round(fieldPosition.y) % vfY)) % vfY;

        return new int[] {x, y};
    }

    private int WrapIndex(int i, float max)
    {
        return (int)((max+(i%max))%max);
    }



    private Vector2 Vec3ToVec2(Vector3 input)
    {
        return new Vector2(input.x, input.z);
    }



    private Vector3 Vec2ToVec3(Vector2 input)
    {
        return new Vector3(input.x, 0, input.y);
    }


    private Vector2 Diffuse(int indexX, int indexY)
    {
        Vector2 output = new Vector2();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                Vector2 velBuffer = vectorfield[WrapIndex(i+indexX, vfX), WrapIndex(j+indexY, vfY)].velBuffer;
                output += velBuffer * Mathf.Max(Vector2.Dot(velBuffer.normalized,  - new Vector2(i,j).normalized), 0);
            }
        }
        return output/2.8f;
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
    public void addForce(Vector2 start, Vector2 end, float factor = 1, int length = 8, int width = 4)
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
                Vector2 force = Vec3ToVec2(endPos - startPos) * factor;
                if (force.magnitude > maxForce)
                {
                    force = force.normalized * maxForce;
                }

                Vector2 direction = (end - start).normalized;
                Vector2 step = Vector2.zero - direction * 4;

                Vector2 widthDirection = Vector2.Perpendicular(step).normalized;

                for (int i = 0; i < length; i++)
                {
                    int x = WrapIndex(index[0] + (int)step.x, vfX);
                    int y = WrapIndex(index[1] + (int)step.y, vfY);
                    vectorfield[x, y].vel = force * Mathf.Lerp(1,0.5f,(float)i/length);

                    Vector2 widthStep = Vector2.zero - widthDirection * (width / 2);

                    for (int j = 0; j < width; j++)
                    {
                        int wx = WrapIndex((int)(x+widthStep.x),vfX);
                        int wy = WrapIndex((int)(y+widthStep.y),vfY);
                        vectorfield[wx, wy].vel = force * Mathf.Lerp(1, 0.5f, (float)i / length);
                        widthStep = widthStep + widthDirection;
                    }

                    step = step + direction;
                }
            }
            catch (System.IndexOutOfRangeException e) {
                print("Index out of range in addForce of Vector Field");
                print(e.Message);
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

    public void resetVectorfield()
    {
        for (int i = 0; i < vfX; i++)
        {
            for (int j = 0; j < vfY; j++)
            {
                vectorfield[i, j] = new VectorfieldFraction();
            }
        }
    }

    public Vector3[] getCameraFloorFrustum()
    {
        return corners;
    }

}



public class VectorfieldFraction
{
    public float pressure;
    public Vector2 vel;
    public Vector2 velBuffer;

    public VectorfieldFraction()
    {
        pressure = 0f;
        vel = new Vector2();
        velBuffer = new Vector2();
    }
}
