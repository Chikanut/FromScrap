float3 bump3y (float3 x, float3 yoffset)
{
    float3 y = 1 - x * x;
    y = saturate(y-yoffset);
    return y;
}
float3 spectral_zucconi6 (float w)
{

 float x = saturate((w - 400.0)/ 300.0);
 
 const float3 cs = float3(3.54541723f, 2.86670055f, 2.29421995f);
 const float3 xs = float3(0.69548916f, 0.49416934f, 0.28269708f);
 const float3 ys = float3(0.02320775f, 0.15936245f, 0.53520021f);
 
 return bump3y ( cs * (x - xs), ys);
}

void Color_float(float u, float d, out float3 color)
{
 color = 0;
 for (int n = 1; n <= 8; n++)
 {
 	float wavelength = u * d / n;
 	color += spectral_zucconi6(wavelength);
 }
  color = saturate(color);
}
