# Flappy Bird AI – NEAT Training in Unity

This project implements an AI system for Flappy Bird using a **NEAT** (NeuroEvolution of Augmenting Topologies) inspired algorithim to train the neural network. The AI learns to play by evolving over generations, improving its performance through **mutation, selection, and elitism**.

---

## Key Features

### AI Training Process
- Spawns a population of AI birds with randomly initialized neural networks.
- Evaluates each bird's performance over multiple cycles to compute **fitness** (survival and progress through pipes).
- Uses **elitism** to retain top performers and **mutation** to generate new behaviors for the next generation.
- Training continues until a bird achieves a predefined fitness threshold.
- Saves the best-performing AI brain for use in gameplay.

### Player vs AI Mode
- Players can compete against pre-trained AI birds at different difficulty levels.
![Training Demo](Assets/watchingBird.gif)
### Watching Mode
- Observe AI birds trained through NEAT to evaluate and compare performance.

---

## Training Highlights
- **Population-based evolution**: Multiple AI birds are trained simultaneously to maximize learning efficiency.
- **Fitness averaging**: Birds are evaluated over several cycles to ensure consistent performance before evolving.
- **Early stopping**: Training can end early if a bird reaches a high fitness, saving computation time.
- **Neural network mutation**: Both weights and biases are mutated at a controlled rate to explore new behaviors.
- **Persistent brains**: Best-performing neural networks are saved as JSON files for later use.

---

## Technical Details
- **Neural Network Structure**: 4 inputs → 5 hidden → 1 output  
- **Inputs**: Bird Y position, velocity, distance to next pipe (horizontal & vertical)  
- **Output**: Flap decision  
- **Activation Function**: Hyperbolic tangent (`tanh`)  
- **Programming**: Unity (C#), `JsonUtility` for saving/loading networks  
- **Optimization**: Adjustable time scale for faster training
