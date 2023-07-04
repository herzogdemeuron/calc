export function getHslColor(label, data, labelKey) {
    const item = data.find((item) => item[labelKey] === label);
    const color = item.color;
    const hslColor = `hsl(${color.Hue}, ${color.Saturation + 5}%, ${color.Lightness - 5}%)`;
    return hslColor;
  }

export function createHslGradient(count) {
    const saturation = 40;
    const lightness = 60;
    // create random start angle between 330 and 380
    const startAngle = Math.floor(Math.random() * 50) + 330;
    const e = 2.718;

    // Sigmoid function
    const sigmoid = x => 1 / (1 + Math.exp(-x));

    // Create upper limit using sigmoid function
    const ePowerCount = Math.pow(e, count);
    let angle = sigmoid(ePowerCount / (ePowerCount + 1));

    // Shift to 0 and stretch to 1
    angle = (angle - 0.5) * 2;

    // Remap to 80 degrees
    angle = angle * 80;

    const angleStep = angle / count;

    const hslColors = [];
    for (let i = 0; i < count; i++) {
      const hue = Math.floor(angleStep * i) + startAngle;
      const hslColor = `hsl(${hue}, ${saturation}%, ${lightness}%)`;
      hslColors.push(hslColor);
    }

    return hslColors;
  }