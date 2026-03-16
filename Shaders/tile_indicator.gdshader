shader_type spatial;

void fragment() {
	float x = UV.x;
	float y = UV.y;

	float s = x + y;
	float dx = x - y;
	float dy = y - x;

	bool outline = (x < 0.05 || x > 0.95 || y < 0.05 || y > 0.95)
		&& !(x > 0.2 && x < 0.8)
		&& !(y > 0.2 && y < 0.8)
		&& !(s < 0.1f || dx > 0.9f || dy > 0.9f || s > 1.9f);

	bool inset =
		(s  > 0.1f  && s  < 0.17f)
		|| (dx < 0.9f  && dx > 0.83f)
		|| (dy < 0.9f  && dy > 0.83f)
		|| (s  < 1.9f  && s  > 1.83f);

	bool show = outline || inset;
	if (show) {
		ALPHA = 1.0f;
	} else {
		ALPHA = 0.0f;
	}

	ALBEDO = vec3(1,1,1);
}