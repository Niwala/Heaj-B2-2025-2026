
void SlimeIntersect_float(float3 ro, float3 rd, float3 ce, float ra, out float2 intersection)
{
    float3 oc = ro - ce;
    float b = dot( oc, rd );
    float c = dot( oc, oc ) - ra*ra;
    float h = b*b - c;
    
    //No intersection
    if( h<0.0 ) 
        intersection = float2(-1.0, -1.0);
        
    h = sqrt( h );
    intersection = float2(-b - h, -b + h);
}