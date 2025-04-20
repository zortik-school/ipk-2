PROJECT_NAME=ipk25-client

CONFIGURATION=Release
OUTPUT_DIR=bin/$(CONFIGURATION)

all: publish

build:
	dotnet build $(PROJECT_NAME).csproj -c $(CONFIGURATION)

run:
	dotnet run --project $(PROJECT_NAME).csproj -c $(CONFIGURATION)

clean:
	dotnet clean $(PROJECT_NAME).csproj

publish:
	dotnet publish $(PROJECT_NAME).csproj -c $(CONFIGURATION) -r linux-x64 --self-contained true /p:PublishSingleFile=true -o $(OUTPUT_DIR)/publish
	cp $(OUTPUT_DIR)/publish/$(PROJECT_NAME) ./ipk25-client

test:
	dotnet test

.PHONY: all build run clean publish test