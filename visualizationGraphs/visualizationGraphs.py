import json
import matplotlib.pyplot as plt

def lineplot(x_data, y_data, y_data2, x_label="", y_label="", title=""):
    _, ax = plt.subplots()
    
    plt.plot(x_data, y_data, 'bo')
    plt.plot(x_data, y_data2, 'k')
    ax.set_title(title)
    ax.set_xlabel(x_label)
    ax.set_ylabel(y_label)

    plt.show()


with open("D:\\лабы\\6 семестр\\ЧМ\\1.2laba\\Save\\temp.json", "r") as read_file:
    data = json.load(read_file)
lineplot(data['X'], data['Y'], data['ApproxValues'])