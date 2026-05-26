import matplotlib.pyplot as plt
import numpy as np

def plot_training_results(data_file):
    # This is a template script.
    # Replace this with the actual loading logic for your training logs.
    # Assuming the data is in a format like: [timestep, reward]
    try:
        data = np.loadtxt(data_file, delimiter=',')
        
        plt.figure(figsize=(10, 5))
        plt.plot(data[:, 0], data[:, 1], label='Mean Reward')
        plt.xlabel('Timestep')
        plt.ylabel('Reward')
        plt.title('Training Progress')
        plt.legend()
        plt.grid(True)
        
        # Save plot to a file
        plt.savefig('training_progress.png')
        print("Plot saved as training_progress.png")
        
    except Exception as e:
        print(f"Error plotting data: {e}")

if __name__ == "__main__":
    # Example usage:
    # plot_training_results('training_logs.csv')
    print("Please provide a path to a valid CSV file containing training logs (e.g., [timestep, reward]).")
