# Název projektu (změň dle potřeby)
PROJECT_NAME=IPK_2

# Výchozí konfigurace a adresář výstupu
CONFIGURATION=Release
OUTPUT_DIR=bin/$(CONFIGURATION)

# Defaultní cíl
all: build

# Build projektu
build:
	dotnet build $(PROJECT_NAME).csproj -c $(CONFIGURATION)

# Spuštění projektu
run:
	dotnet run --project $(PROJECT_NAME).csproj -c $(CONFIGURATION)

# Vyčištění výstupních souborů
clean:
	dotnet clean $(PROJECT_NAME).csproj

# Publikace aplikace
publish:
	dotnet publish $(PROJECT_NAME).csproj -c $(CONFIGURATION) -o $(OUTPUT_DIR)/publish

# Testování (pokud máš testovací projekt)
test:
	dotnet test

.PHONY: all build run clean publish test