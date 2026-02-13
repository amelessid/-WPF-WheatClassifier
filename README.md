# WheatClassifier

Console application implementing a k-Nearest Neighbors (k-NN) classifier optimized using a KD-Tree.

---

## 📌 Project Description

This project classifies wheat grains into three varieties:

- Canadian  
- Kama  
- Rosa  

The classification is performed using the k-NN algorithm with an optimized nearest neighbor search based on a KD-Tree structure.

The dataset contains seven numerical features describing each grain.

---

## ⚙️ Main Features

- KD-Tree construction for efficient neighbor search
- k-NN classifier with configurable k value
- Euclidean and Manhattan distance metrics
- Custom MergeSort implementation (no built-in sort used)
- Accuracy calculation
- Confusion matrix display (real × predicted)
- JSON export of results
- Interactive console interface using Spectre.Console

---

## 📊 Evaluation

After classification, the program displays:

- The accuracy (percentage of correct predictions)
- The confusion matrix (real class vs predicted class)

The results are also saved in a JSON file.

---

## ▶ How to Run

1. Open the solution in Visual Studio
2. Build the project
3. Run the application
4. Use the interactive menu to:
   - Choose the value of k
   - Select the distance metric
   - Launch classification

---

## 🎓 Educational Purpose

This project demonstrates:

- Object-Oriented Programming principles
- Use of interfaces and abstraction
- Strategy pattern (distance metrics)
- Recursive data structures (KD-Tree)
- Basic machine learning evaluation techniques
