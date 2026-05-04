shader_type spatial;

uniform vec4 grass_color : source_color = vec4(0.2, 0.8, 0.2, 1.0);
// uniform vec4 grass_color : source_color = vec4(0.2, 0.8, 0.2, 1.0);
uniform vec4 soil_color  : source_color = vec4(0.4, 0.25, 0.1, 1.0);

void fragment() {
    vec3 mask = COLOR.rgb;

    vec3 col =
        grass_color.rgb * mask.g +
        soil_color.rgb  * mask.r;

    ALBEDO = col;
}