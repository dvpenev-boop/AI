const form = document.querySelector("#bmi-form");
const bmiNumberEl = document.querySelector("#bmi-number");
const bmiValueEl = document.querySelector("#bmi-value");
const bmiCategoryEl = document.querySelector("#bmi-category");
const resultSummaryEl = document.querySelector("#result-summary");
const chartEl = document.querySelector("#bmi-chart");
const indicatorEl = document.querySelector("#chart-indicator");

const categories = [
    { label: "Underweight", min: 0, max: 18.5, colorVar: "--underweight", advice: "Consider a nutrient-rich diet to reach a healthy weight." },
    { label: "Normal", min: 18.5, max: 25, colorVar: "--normal", advice: "Keep up the balanced diet and regular physical activity." },
    { label: "Overweight", min: 25, max: 30, colorVar: "--overweight", advice: "Small lifestyle adjustments can help reduce future risk." },
    { label: "Obesity", min: 30, max: 60, colorVar: "--obesity", advice: "Work with a health professional on a sustainable plan." }
];

form.addEventListener("submit", (event) => {
    event.preventDefault();

    const formData = new FormData(form);
    const age = Number(formData.get("age"));
    const gender = formData.get("gender");
    const height = Number(formData.get("height"));
    const weight = Number(formData.get("weight"));

    if (!age || !gender || !height || !weight || height <= 0 || weight <= 0) {
        resultSummaryEl.textContent = "Please fill in all fields with valid values.";
        bmiValueEl.hidden = true;
        chartEl.hidden = true;
        bmiCategoryEl.textContent = "";
        bmiCategoryEl.style.color = "";
        return;
    }

    const bmi = calculateBmi(weight, height);
    const bmiRounded = Number(bmi.toFixed(1));
    const category = resolveCategory(bmiRounded);

    bmiNumberEl.textContent = bmiRounded.toFixed(1);
    bmiValueEl.hidden = false;
    bmiCategoryEl.textContent = `${category.label} BMI`;
    bmiCategoryEl.style.color = `var(${category.colorVar})`;

    const genderLabel = formatGender(gender);
    resultSummaryEl.textContent = `For a ${age}-year-old ${genderLabel} at ${height} cm and ${weight} kg, a BMI of ${bmiRounded.toFixed(1)} is considered ${category.label.toLowerCase()}. ${category.advice}`;

    updateIndicator(bmiRounded);
    chartEl.hidden = false;
});

function calculateBmi(weightKg, heightCm) {
    const heightM = heightCm / 100;
    return weightKg / (heightM * heightM);
}

function resolveCategory(bmi) {
    return categories.find((range) => bmi >= range.min && bmi < range.max) || categories[categories.length - 1];
}

function updateIndicator(bmi) {
    // Map BMI range 12-40 to 0-100%. We clamp values to keep the arrow within the scale.
    const minBmi = 12;
    const maxBmi = 40;
    const clamped = Math.min(Math.max(bmi, minBmi), maxBmi);
    const percent = ((clamped - minBmi) / (maxBmi - minBmi)) * 100;
    indicatorEl.style.left = `${percent}%`;
}

function formatGender(value) {
    if (!value) {
        return "person";
    }
    if (value === "other") {
        return "person";
    }
    return value.charAt(0).toUpperCase() + value.slice(1).toLowerCase();
}
