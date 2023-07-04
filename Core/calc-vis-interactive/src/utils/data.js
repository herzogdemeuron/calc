import { getHslColor, createHslGradient, extractHueFromHslColor } from './colors.js';


function preprocessData(data, labelKey, valueKey) {
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

export function getChartData(data, labelKey, valueKey, sort) {
  const processedData = preprocessData(data, labelKey, valueKey);
  if (sort === true){
    processedData.sort((a, b) => b[valueKey] - a[valueKey]);
  }
  let labels = processedData.map((item) => item[labelKey]);
  let values = processedData.map((item) => item[valueKey]);
  let colors = [];
  // get HslColor for each label
  if (labelKey === 'buildup_name') {
    for (let label of labels) {
      const hslColor = getHslColor(label, data, labelKey);
      colors.push(hslColor);
    }
    // sort colors by hue, sort labels and values in parallel use
    const sortedColors = colors.sort((a, b) => extractHueFromHslColor(a) - extractHueFromHslColor(b));
    const sortedLabels = [];
    const sortedValues = [];
    for (let color of sortedColors) {
      const index = colors.indexOf(color);
      sortedLabels.push(labels[index]);
      sortedValues.push(values[index]);
    }
    labels = sortedLabels;
    values = sortedValues;
  }
  else {
    colors = createHslGradient(labels.length);
  }
  
  return { labels, values, colors };
}

export function getChartHistoryData(historyData, valueKey) {
  const snapshotTotals = [];
  const labels = new Set();
  for (let snapshot of historyData) {
    // iterate over list of objects
    const data = snapshot.data;
    console.log(snapshot.time)
    console.log(data);
    const snapshotTotal = data.reduce((total, item) => total + item[valueKey], 0);
    snapshotTotals.push(snapshotTotal);
    labels.add(snapshot.time);
  }

  return { labels: Array.from(labels), snapshotTotals };
}