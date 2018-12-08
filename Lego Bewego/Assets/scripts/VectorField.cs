using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorField : MonoBehaviour {

    private Vector2[][] vectorfield = null;

	// Use this for initialization
	void Start () {
		// check how much the camera sees
	}
	
	// Update is called once per frame
	void Update () {
		
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
