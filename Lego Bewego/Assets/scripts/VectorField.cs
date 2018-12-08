using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorField : MonoBehaviour {

    public static VectorField instance;

    public List<Rigidbody> particles;

    public float forceFactor = 50;


    private Vector2[,] vectorfield;
    // store the current extents of the vectorfield
    private Vector2 vectorfieldOrigin;
    private Vector2 vectorfieldOffset;

    // size of array 
    private int vX = 0;
    private int vY = 0;

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
        vX = (int)(vectorfieldMax.x - vectorfieldOrigin.x);
        vY = (int)(vectorfieldMax.y - vectorfieldOrigin.y);
        vectorfield = new Vector2[vX, vY];

        for(int i=0; i<vX; i++)
        {
            for (int j = 0; j < vY; j++)
            {
                vectorfield[i, j] = new Vector2(Random.Range(-1f,1f), Random.Range(-1f, 1f));
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
            for (int i = 0; i < vY; i++)
            {
                vectorfield[(int)(vX + ((vectorfieldOffset.x - 1) % vX)) % vX, i] = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
        }

        if (newOrigin.x < vectorfieldOrigin.x + vectorfieldOffset.x)
        {
            // reduce Offset
            vectorfieldOffset.Set(vectorfieldOffset.x - 1, vectorfieldOffset.y);

            // clear fields
            for (int i = 0; i < vectorfield.GetLength(1); i++)
            {
                vectorfield[(int)(vX + ((vectorfieldOffset.x - 1)%vX)) % vX, i] = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
        }

        if (newOrigin.y > vectorfieldOrigin.y + vectorfieldOffset.y)
        {
            // raise Offset
            vectorfieldOffset.Set(vectorfieldOffset.x, vectorfieldOffset.y+1);

            // clear fields
            for (int i = 0; i < vX; i++)
            {
                vectorfield[i, (int)(vY + ((vectorfieldOffset.y - 1) % vY)) % vY] = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
        }

        if (newOrigin.y < vectorfieldOrigin.y + vectorfieldOffset.y)
        {
            // raise Origin
            vectorfieldOffset.Set(vectorfieldOffset.x, vectorfieldOffset.y - 1);

            // clear fields
            for (int i = 0; i < vX; i++)
            {
                vectorfield[i, (int)(vY + ((vectorfieldOffset.y - 1) % vY)) % vY] = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
        }


        // force propagation
        /*for all cells (x,y) {
      vx(x,y) += ( p(x-1,y  ) - p(x+1,y  ) )*0.5;
      vy(x,y) += ( p(x  ,y-1) - p(x  ,y+1) )*0.5;

      if ( frictionTurnedOn ) {
         vx(x,y) *= 0.99;
         vy(x,y) *= 0.99;
      }
   }
         * 
         */
        for (int i = 0; i < vX; i++)
        {
            for (int j = 0; j < vY; j++)
            {
                // damp
                /*
                 * */
                vectorfield[i, j].Set(
                    vectorfield[i, j].x * 0.98f, 
                    vectorfield[i, j].y * 0.98f
                    );

                // diffuse
                /*
                vectorfield[i, j].Set(
                    (float) (vectorfield[i, j].x + ((vectorfield[Mathf.Max(i - 1, 0), j].x - vectorfield[Mathf.Min(i + 1,vX), j].x) * 0.5) * 0.95),
                    (float) (vectorfield[i, j].y + ((vectorfield[i, Mathf.Max(j - 1, 0)].y - vectorfield[i, Mathf.Min(j + 1, vY)].y) * 0.5) * 0.95)
                    );
                    */

            }
        }


        // add force to particles
        foreach(Rigidbody particle in particles)
        {
            particle.AddForce(forceFactor * getForce(particle.GetComponent<Transform>().position));
        }




        if (Input.GetMouseButtonDown(0) && DebugManager.instance.debugVectorField)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float enter = 0f;

            if (ground.Raycast(ray, out enter))
            {
                Vector3 pos = ray.GetPoint(enter);
            }
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

            for (int i = 0; i < vX; i++)
            {
                for (int j = 0; j < vY; j++)
                {
                    Vector2 shift = new Vector2(
                        Mathf.Floor((vX+vectorfieldOffset.x-i) / vX), 
                        Mathf.Floor((vY+vectorfieldOffset.y-j) / vY));

                    Vector3 start = new Vector3(
                        i + shift.x*vX + vectorfieldOrigin.x,
                        0,
                        j + shift.y*vY + vectorfieldOrigin.y);

                    Gizmos.DrawWireCube(start, new Vector3(1, 0.01f, 1));
                    Debug.DrawLine(start, start + new Vector3(vectorfield[i, j].x, 0, vectorfield[i, j].y));
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
        Vector2 fieldPosition = new Vector2(position.x, position.z) - vectorfieldOrigin;
        int x = (int)(Mathf.Round(fieldPosition.x) % vX);
        int y = (int)(Mathf.Round(fieldPosition.y) % vY);
        if (x < 0 || x > vX || y < 0 || y > vY) throw new System.IndexOutOfRangeException();
        return new int[] {Mathf.Clamp(x, 0, vX), Mathf.Clamp(y, 0, vY)};
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
        return Vec2ToVec3(vectorfield[x,y]);
    }


    // input pixel coordinates, raycast to ground and add force
    public void addForce(Vector2 start, Vector2 end)
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
                vectorfield[index[0], index[1]] = force;
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

}


/* Code Reference
 * PGraphics heightfield;
final int heightfieldDefaultValue = 255;

// creates a vectorfield with a field size given in SIZE
// the direction of each is dependant of the slope specified in heightfield 
class VectorField{
  
  // DEBUG
  public boolean drawHeightfield = false;
  public boolean drawVectors = false;

  // size of a vector square in pixels
  private final int SIZE = 30/shrink;
  
  // scale each vector by this factor
  private final float VSCALE = 1;
  
  private int fieldCountX = (int)(WindowWidth/SIZE +1);
  private int fieldCountY = (int)(FloorHeight/SIZE +1);
  
  private PVector[][] vectors = new PVector[fieldCountX][fieldCountY];
  
  private int blurTimestamp = 0;
  private final int BLUR_FREQ = 5000;
  
  
  public VectorField(){
      heightfield = createGraphics(fieldCountX, fieldCountY);
      
      heightfield.beginDraw();
      heightfield.background(heightfieldDefaultValue);
      heightfield.endDraw();
      heightfield.blendMode(MULTIPLY);
  }
  
  
  // drawing is for debug only and slows down rendering significantly
  void draw(){
    //draw heightfield
    if(drawHeightfield){
      image(heightfield,0,WallHeight, WindowWidth, FloorHeight);
    } //<>//
    // draw the vectors representing the direction and strength
    if(drawVectors){
      stroke(0);
      strokeWeight(1);
      for (int x = 0; x<fieldCountX; x++){
        for (int y = 0; y<fieldCountY; y++){
          float centerX = x * SIZE + SIZE/2;
          float centerY = y * SIZE + SIZE /2 + WallHeight;
          PVector dir = vectors[x][y].copy();
          dir.mult(0.1).limit(SIZE/2);
          line(centerX, centerY, 
            centerX + dir.x, 
            centerY + dir.y);
        }
      }
    }
  }
  
  
  public void update(){
    heightfield.loadPixels();
    // blur heightfield every 2 seconds
    
    if(millis() - blurTimestamp > BLUR_FREQ){
      //TODO write own blur that is not that strong
      int[] heightfieldBuffer = heightfield.pixels.clone();
      shiftBlur1(heightfieldBuffer, heightfield.pixels);
      heightfield.updatePixels();
      blurTimestamp = millis();
    }
    
    // set the vector to the direction of the lowest neighbouring value
    for (int x = 0; x<fieldCountX; x++){
      for (int y = 0; y<fieldCountY; y++){
        vectors[x][y] = findDownVector(x, y);
      }
    }
  }
  

  private void shiftBlur1(int[] s, int[] t) { // source & target buffer
    int yOffset = 0;
    int w = heightfield.width;
    int h = heightfield.height;
    for (int i = 1; i < (w-1); ++i) {
  
      yOffset = w*(h-1);
      // top edge (minus corner pixels)
      t[i] = (((((s[i] & 0xFF) << 2 ) + 
        (s[i+1] & 0xFF) + 
        (s[i-1] & 0xFF) + 
        (s[i + w] & 0xFF) + 
        (s[i + yOffset] & 0xFF)) >> 3)  & 0xFF) +
        (((((s[i] & 0xFF00) << 2 ) + 
        (s[i+1] & 0xFF00) + 
        (s[i-1] & 0xFF00) + 
        (s[i + w] & 0xFF00) + 
        (s[i + yOffset] & 0xFF00)) >> 3)  & 0xFF00) +
        (((((s[i] & 0xFF0000) << 2 ) + 
        (s[i+1] & 0xFF0000) + 
        (s[i-1] & 0xFF0000) + 
        (s[i + w] & 0xFF0000) + 
        (s[i + yOffset] & 0xFF0000)) >> 3)  & 0xFF0000) +
        0xFF000000; //ignores transparency
  
      // bottom edge (minus corner pixels)
      t[i + yOffset] = (((((s[i + yOffset] & 0xFF) << 2 ) + 
        (s[i - 1 + yOffset] & 0xFF) + 
        (s[i + 1 + yOffset] & 0xFF) +
        (s[i + yOffset - w] & 0xFF) +
        (s[i] & 0xFF)) >> 3) & 0xFF) +
        (((((s[i + yOffset] & 0xFF00) << 2 ) + 
        (s[i - 1 + yOffset] & 0xFF00) + 
        (s[i + 1 + yOffset] & 0xFF00) +
        (s[i + yOffset] & 0xFF00) +
        (s[i] & 0xFF00)) >> 3) & 0xFF00) +
        (((((s[i + yOffset] & 0xFF0000) << 2 ) + 
        (s[i - 1 + yOffset] & 0xFF0000) + 
        (s[i + 1 + yOffset] & 0xFF0000) +
        (s[i + yOffset - w] & 0xFF0000) +
        (s[i] & 0xFF0000)) >> 3) & 0xFF0000) +
        0xFF000000;    
  
      // central square
      for (int j = 1; j < (h-1); ++j) {
        yOffset = j*w;
        t[i + yOffset] = (((((s[i + yOffset] & 0xFF) << 2 ) +
          (s[i + 1 + yOffset] & 0xFF) +
          (s[i - 1 + yOffset] & 0xFF) +
          (s[i + yOffset + w] & 0xFF) +
          (s[i + yOffset - w] & 0xFF)) >> 3) & 0xFF) +
          (((((s[i + yOffset] & 0xFF00) << 2 ) +
          (s[i + 1 + yOffset] & 0xFF00) +
          (s[i - 1 + yOffset] & 0xFF00) +
          (s[i + yOffset + w] & 0xFF00) +
          (s[i + yOffset - w] & 0xFF00)) >> 3) & 0xFF00) +
          (((((s[i + yOffset] & 0xFF0000) << 2 ) +
          (s[i + 1 + yOffset] & 0xFF0000) +
          (s[i - 1 + yOffset] & 0xFF0000) +
          (s[i + yOffset + w] & 0xFF0000) +
          (s[i + yOffset - w] & 0xFF0000)) >> 3) & 0xFF0000) +
          0xFF000000;
      }
    }
  
    // left and right edge (minus corner pixels)
    for (int j = 1; j < (h-1); ++j) {
      yOffset = j*w;
      t[yOffset] = (((((s[yOffset] & 0xFF) << 2 ) +
        (s[yOffset + 1] & 0xFF) +
        (s[yOffset + w - 1] & 0xFF) +
        (s[yOffset + w] & 0xFF) +
        (s[yOffset - w] & 0xFF) ) >> 3) & 0xFF) +
        (((((s[yOffset] & 0xFF00) << 2 ) +
        (s[yOffset + 1] & 0xFF00) +
        (s[yOffset + w - 1] & 0xFF00) +
        (s[yOffset + w] & 0xFF00) +
        (s[yOffset - w] & 0xFF00) ) >> 3) & 0xFF00) +
        (((((s[yOffset] & 0xFF0000) << 2 ) +
        (s[yOffset + 1] & 0xFF0000) +
        (s[yOffset + w - 1] & 0xFF0000) +
        (s[yOffset + w] & 0xFF0000) +
        (s[yOffset - w] & 0xFF0000) ) >> 3) & 0xFF0000) +
        0xFF000000;
  
      t[yOffset + w - 1] = (((((s[(j+1)*w - 1] & 0xFF) << 2 ) +
        (s[j*w] & 0xFF) +
        (s[yOffset + w - 2] & 0xFF) +
        (s[yOffset + (w<<1) - 1] & 0xFF) +
        (s[yOffset - 1] & 0xFF)) >> 3) & 0xFF) +
        (((((s[yOffset + w - 1] & 0xFF00) << 2) +
        (s[yOffset] & 0xFF00) +
        (s[yOffset + w - 2] & 0xFF00) +
        (s[yOffset + (w<<1) - 1] & 0xFF00) +
        (s[yOffset - 1] & 0xFF00)) >> 3) & 0xFF00) +
        (((((s[yOffset + w - 1] & 0xFF0000) << 2) +
        (s[yOffset] & 0xFF0000) +
        (s[yOffset + w - 2] & 0xFF0000) +
        (s[yOffset + (w<<1) - 1] & 0xFF0000) +
        (s[yOffset - 1] & 0xFF0000)) >> 3) & 0xFF0000) +
        0xFF000000;
    }
  
    // corner pixels
    t[0] = (((((s[0] & 0xFF) << 2) + 
      (s[1] & 0xFF) + 
      (s[w-1] & 0xFF) + 
      (s[w] & 0xFF) + 
      (s[w*(h-1)] & 0xFF)) >> 3)  & 0xFF) +
      (((((s[0] & 0xFF00) << 2) + 
      (s[1] & 0xFF00) + 
      (s[w-1] & 0xFF00) + 
      (s[w] & 0xFF00) + 
      (s[w*(h-1)] & 0xFF00)) >> 3)  & 0xFF00) +
      (((((s[0] & 0xFF0000) << 2) + 
      (s[1] & 0xFF0000) + 
      (s[w-1] & 0xFF0000) + 
      (s[w] & 0xFF0000) + 
      (s[w*(h-1)] & 0xFF0000)) >> 3)  & 0xFF0000) +
      0xFF000000;
  
    t[w - 1 ] = (((((s[w-1] & 0xFF) << 2) + 
      (s[w-2] & 0xFF) + 
      (s[0] & 0xFF) + 
      (s[(w<<1) - 1] & 0xFF) + 
      (s[w*h-1] & 0xFF) ) >> 3) & 0xFF) +
      (((((s[w-1] & 0xFF00) << 2) + 
      (s[w-2] & 0xFF00) + 
      (s[0] & 0xFF00) + 
      (s[(w<<1) - 1] & 0xFF00) + 
      (s[w*h-1] & 0xFF00) ) >> 3) & 0xFF00) +
      (((((s[w-1] & 0xFF0000) << 2) + 
      (s[w-2] & 0xFF0000) + 
      (s[0] & 0xFF0000) + 
      (s[(w<<1) - 1] & 0xFF0000) + 
      (s[w*h-1] & 0xFF0000) ) >> 3) & 0xFF0000) +
      0xFF000000;
  
    t[w * h - 1] = (((((s[w*h-1] & 0xFF) << 2) + 
      (s[w-1] & 0xFF) + 
      (s[w*(h-1)-1] & 0xFF) + 
      (s[w*h-2] & 0xFF) + 
      (s[w*(h-1)] & 0xFF) ) >> 3) & 0xFF) +
      (((((s[w*h-1] & 0xFF00) << 2) + 
      (s[w-1] & 0xFF00) + 
      (s[w*(h-1)-1] & 0xFF00) + 
      (s[w*h-2] & 0xFF00) + 
      (s[w*(h-1)] & 0xFF00) ) >> 3) & 0xFF00) +
      (((((s[w*h-1] & 0xFF0000) << 2) + 
      (s[w-1] & 0xFF0000) + 
      (s[w*(h-1)-1] & 0xFF0000) + 
      (s[w*h-2] & 0xFF0000) + 
      (s[w*(h-1)] & 0xFF0000) ) >> 3) & 0xFF0000) +
      0xFF000000;
  
    t[w *(h-1)] = (((((s[w*(h-1)] & 0xFF) << 2) + 
      (s[w*(h-1) + 1] & 0xFF) + 
      (s[w*h-1] & 0xFF) + 
      (s[w*(h-2)] & 0xFF) + 
      (s[0] & 0xFF) ) >> 3) & 0xFF) +
      (((((s[w*(h-1)] & 0xFF00) << 2) + 
      (s[w*(h-1) + 1] & 0xFF00) + 
      (s[w*h-1] & 0xFF00) + 
      (s[w*(h-2)] & 0xFF00) + 
      (s[0] & 0xFF00) ) >> 3) & 0xFF00) +
      (((((s[w*(h-1)] & 0xFF0000) << 2) + 
      (s[w*(h-1) + 1] & 0xFF0000) + 
      (s[w*h-1] & 0xFF0000) + 
      (s[w*(h-2)] & 0xFF0000) + 
      (s[0] & 0xFF0000) ) >> 3) & 0xFF0000) +
      0xFF000000;
  }
  
  // check the neighbouring fields of the heights array 
  // combine all delta values to get a final vector 
  private PVector findDownVector(int x, int y){
    PVector downVec = new PVector();
    
    int dx = 0;
    int dy = 0;
    int w = heightfield.width;
    int center = (int)red(heightfield.pixels[x + y * w]);
    
    // check top left
    if(y > 0 && x > 0){
      dx -= center - (int)red(heightfield.pixels[x-1 + (y-1)*w]);
      dy -= center - (int)red(heightfield.pixels[x-1 + (y-1)*w]);
    }
    // check top
    if (y > 0){
      dy -= center - (int)red(heightfield.pixels[x + (y-1)*w]);
    }
    // check top right
    if(y > 0 && x < fieldCountX -1){
      dx += center - (int)red(heightfield.pixels[x+1 + (y-1)*w]);
      dy -= center - (int)red(heightfield.pixels[x+1 + (y-1)*w]);
    }
    // check left
    if(x > 0){
      dx -= center - (int)red(heightfield.pixels[x-1 + y*w]);
    }
    // check right
    if(x < fieldCountX-1){
      dx += center - (int)red(heightfield.pixels[x+1 + y*w]);
    }
    // check bottom left
    if(x >0 && y < fieldCountY -1){
      dx -= center - (int)red(heightfield.pixels[x-1 + (y+1)*w]);
      dy += center - (int)red(heightfield.pixels[x-1 + (y+1)*w]);
    }
    // check bottom
    if(y < fieldCountY-1){
      dy += center - (int)red(heightfield.pixels[x + (y+1)*w]);
    }
    // check bottom right
    if(x < fieldCountX -1 && y < fieldCountY -1){
      dx += center - (int)red(heightfield.pixels[x+1 + (y+1)*w]);
      dy += center - (int)red(heightfield.pixels[x+1 + (y+1)*w]);
    }
    

    downVec.set(dx, dy);
    
    return downVec;
  }
  
  
  
  // heightfield fading
  // better version than just overlaying a rect with alpha, 
  // because it leaves no traces
  private void fadeHeightfield(){
    heightfield.loadPixels();
      for (int i = 0; i < width * height; i++){
        float hfVal = red(heightfield.pixels[i]);
        float delta = hfVal- heightfieldDefaultValue;
        if (delta != 0){
          float sign = delta / abs(delta);
          int newVal = (int)(hfVal - sign* ceil(abs(delta)/8));
          heightfield.pixels[i] =  color(newVal);
        }
      }
    heightfield.updatePixels();
  }
  
  
  
  // returns the vector for the given position multiplied by VSCALE
  // no searching because I can calculate the right vectorfield based on position
  public PVector getAcc(PVector pos){
    int x = (int)(pos.x/SIZE);
    int y = (int)(pos.y/SIZE);
    if (x<fieldCountX && y < fieldCountY && x>=0 && y>=0){
      return PVector.mult(vectors[x][y],VSCALE);
    }
    return new PVector();
  }
  
  public int getHeight(PVector pos){
    int x = (int)pos.x/SIZE;
    int y = (int)pos.y/SIZE;
    if (x<fieldCountX && y < fieldCountY && x>=0 && y>=0){
      return (int)red(heightfield.pixels[x + y * heightfield.width]);
    }
    return 255;
  }
  
  public void reset(){
    heightfield.beginDraw();
    heightfield.background(heightfieldDefaultValue);
    heightfield.endDraw();
  }
  
  public void displace(Object[] players){
    heightfield.beginDraw();
    heightfield.noStroke();
    for(Object op : players){
      Player p = (Player)op;
      PVector pos = p.getPosition();
      heightfield.fill(p.getDisplacement());
      heightfield.ellipse(pos.x/SIZE,(pos.y-WallHeight)/SIZE,1,1);
    }
    heightfield.endDraw();
  }
  
  
}
 * 
 * 
 */
