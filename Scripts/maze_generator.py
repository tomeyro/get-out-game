import random

grid_size = 10
grid_range = range(0, grid_size)
grid = [[0] * grid_size for x in grid_range]
connections = [[[] for x in grid_range] for y in grid_range]

maze = ''
for cell_row in grid_range:
    for cell_col in grid_range:
        maze += '%s  ' % grid[cell_row][cell_col]
    maze += '\n'
print(maze)

first_cell = [random.randint(0, grid_size - 1), random.randint(0, grid_size - 1)]
grid[first_cell[0]][first_cell[1]] += 1
trackback = [first_cell]
print(first_cell)
print('\n')

def get_next_cell(cell):
    available_cells = []
    # Check top
    if cell[0] - 1 >= 0 and not grid[cell[0] - 1][cell[1]]:
        available_cells.append([cell[0] - 1, cell[1]])
    # Check bottom
    if cell[0] + 1 < grid_size and not grid[cell[0] + 1][cell[1]]:
        available_cells.append([cell[0] + 1, cell[1]])
    # Check left
    if cell[1] - 1 >= 0 and not grid[cell[0]][cell[1] - 1]:
        available_cells.append([cell[0], cell[1] - 1])
    # Check right
    if cell[1] + 1 < grid_size and not grid[cell[0]][cell[1] + 1]:
        available_cells.append([cell[0], cell[1] + 1])
    if not available_cells:
        return False
    return available_cells[random.randint(0, len(available_cells) - 1)]

while trackback:
    current_cell = trackback[-1]
    # print((' ' * (len(trackback) - 1)) + str(current_cell))
    next_cell = get_next_cell(current_cell)
    if next_cell:
        if next_cell not in connections[current_cell[0]][current_cell[1]]:
            connections[current_cell[0]][current_cell[1]].append(next_cell)
        if current_cell not in connections[next_cell[0]][next_cell[1]]:
            connections[next_cell[0]][next_cell[1]].append(current_cell)

        grid[next_cell[0]][next_cell[1]] += 1
        trackback.append(next_cell)
        continue
    trackback.pop(-1)
print('')

maze = ''
for cell_row in grid_range:
    for cell_col in grid_range:
        conn = ''
        conn += '↑' if [cell_row - 1, cell_col] in connections[cell_row][cell_col] else '·'
        conn += '→' if [cell_row, cell_col + 1] in connections[cell_row][cell_col] else '·'
        conn += '↓' if [cell_row + 1, cell_col] in connections[cell_row][cell_col] else '·'
        conn += '←' if [cell_row, cell_col - 1] in connections[cell_row][cell_col] else '·'
        maze += '%s  ' % conn
    maze += '\n'
print(maze)

maze = ''
for cell_row in grid_range:
    for cell_col in grid_range:
        maze += '%s  ' % grid[cell_row][cell_col]
    maze += '\n'
print(maze)
