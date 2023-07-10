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
    const startAngle = 180;
    const endAngle = 300;
    const angleRange = endAngle - startAngle;
    const angleIncrement = angleRange / count;
    const hslColors = [];
    for (let i = 0; i < count; i++) {
        const hue = startAngle + (angleIncrement * i);
        const hslColor = `hsl(${hue}, ${saturation}%, ${lightness}%)`;
        hslColors.push(hslColor);
    }
    return hslColors;
  }

export function extractHueFromHslColor(hslColor) {
    const regex = /hsl\((\d+),/;
    const match = hslColor.match(regex);
    if (match) {
      return parseInt(match[1]);
    }
    return 0; // Default hue value if extraction fails
  }