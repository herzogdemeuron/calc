import { getHslColor, createHslGradient } from './colors.js';

export function preprocessData(data, labelKey, valueKey) {
    const groupedData = {};

    // Group data by key
    data.forEach((item) => {
      const key = item[labelKey];
      const value = item[valueKey];

      if (Object.prototype.hasOwnProperty.call(groupedData, key)) {
        groupedData[key] += value;
      } else {
        groupedData[key] = value;
      }
    });

    // Convert grouped data into an array of objects
    const processedData = Object.entries(groupedData).map(([key, value]) => ({
      [labelKey]: key,
      [valueKey]: value
    }));

    return processedData;
  }

export function getChartData(data, labelKey, valueKey) {
  const processedData = preprocessData(data, labelKey, valueKey);
  const labels = processedData.map((item) => item[labelKey]);
  const values = processedData.map((item) => item[valueKey]);
  let colors = [];
  // get HslColor for each label
  if (labelKey === 'buildup_name') {
    colors = labels.map((label) => getHslColor(label, data, labelKey));
  }
  else {
    colors = createHslGradient(labels.length);
  }
  
  return {
    labels: labels,
    datasets: [
      {
        data: values,
        backgroundColor: colors,
        borderRadius: 10,
        borderSkipped: false,
      }
    ]
  };
}