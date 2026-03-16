shader_type spatial;

void fragment() {
	float x = UV.x;
	float y = UV.y;

	bool show =
		// Basic square outline
		(x < 0.05 || x > 0.95 || y < 0.05 || y > 0.95)

		// Remove middle straights
		&& !(x > 0.2 && x < 0.8)
		&& !(y > 0.2 && y < 0.8)

		// Bevel corners
		&& !((x + y) < 0.1f)
		&& !((x - y) > 0.9f)
		&& !((y - x) > 0.9f)
		&& !((x + y) > 1.9f)

		// Inset corners
		|| (x + y) > 0.1f && (x + y) < 0.17f
		|| (x - y) < 0.9f && (x - y) > 0.83f
		|| (y - x) < 0.9f && (y - x) > 0.83f
		|| (x + y) < 1.9f && (x + y) > 1.83f
		;
	if (show) {
		ALPHA = 1.0f;
	} else {
		ALPHA = 0.0f;
	}

	ALBEDO = vec3(1,1,1);
}