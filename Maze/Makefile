CC = dotnet

SOLUTION = Maze.sln
APP_PROJECT = Maze.Gui/Gui.csproj

NAME = Maze
CONFIG = Release

PUBLISH_DIR = ./publish
INSTALL_DIR ?= ./install

# Detect OS
ifeq ($(OS),Windows_NT)
    RM = powershell -Command "Remove-Item -Recurse -Force"
    CP = powershell -Command "Copy-Item -Recurse -Force"
    MKDIR = powershell -Command "New-Item -ItemType Directory -Force -Path"
    RMDIR = powershell -Command "Remove-Item -Recurse -Force"
    FIND = powershell -Command "Get-ChildItem -Recurse -Directory"
    COPY = powershell -Command "Copy-Item -Force"
    IFEXIST = powershell -Command "Test-Path"
else
    RM = rm -rf
    CP = cp -r
    MKDIR = mkdir -p
    RMDIR = rm -rf
    FIND = find
    COPY = cp
    IFEXIST = test -f
endif

all:
	$(CC) build $(SOLUTION) -c $(CONFIG)

clean:
	$(CC) clean $(SOLUTION)
ifeq ($(OS),Windows_NT)
	@powershell -Command "if (Test-Path '$(PUBLISH_DIR)') { Remove-Item -Recurse -Force '$(PUBLISH_DIR)' -ErrorAction SilentlyContinue }"
	@powershell -Command "if (Test-Path '$(INSTALL_DIR)') { Remove-Item -Recurse -Force '$(INSTALL_DIR)' -ErrorAction SilentlyContinue }"
	@powershell -Command "Get-ChildItem -Recurse -Directory -Filter bin | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue"
	@powershell -Command "Get-ChildItem -Recurse -Directory -Filter obj | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue"
else
	$(RM) $(PUBLISH_DIR)
	$(RM) $(INSTALL_DIR)
	@find . -type d \( -name "bin" -o -name "obj" \) 2>/dev/null | xargs rm -rf 2>/dev/null || true
endif

install: all
	$(CC) publish $(APP_PROJECT) -c $(CONFIG) -o $(PUBLISH_DIR)
ifeq ($(OS),Windows_NT)
	@powershell -Command "if (-not (Test-Path '$(INSTALL_DIR)')) { New-Item -ItemType Directory -Force -Path '$(INSTALL_DIR)' | Out-Null }"
	@powershell -Command "Copy-Item -Path '$(PUBLISH_DIR)\*' -Destination '$(INSTALL_DIR)' -Recurse -Force"
else
	$(MKDIR) $(INSTALL_DIR)
	$(CP) $(PUBLISH_DIR)/* $(INSTALL_DIR)/
endif

uninstall:
ifeq ($(OS),Windows_NT)
	@powershell -Command "if (Test-Path '$(INSTALL_DIR)') { Remove-Item -Recurse -Force '$(INSTALL_DIR)' }"
else
	$(RM) $(INSTALL_DIR)
endif

tests:
	$(CC) test $(SOLUTION) -c $(CONFIG)

dvi:
ifeq ($(OS),Windows_NT)
	@powershell -Command "if (-not (Get-Command doxygen -ErrorAction SilentlyContinue)) { Write-Host 'doxygen not found. Please install doxygen.'; exit 1 }; if (-not (Test-Path Doxyfile)) { Write-Host 'Doxyfile not found. Generating template...'; doxygen -g Doxyfile }; Get-ChildItem -Recurse -Include *.cs -Exclude bin,obj | ForEach-Object { $$content = [System.IO.File]::ReadAllText($$_.FullName, [System.Text.Encoding]::GetEncoding('Windows-1251')); [System.IO.File]::WriteAllText($$_.FullName, $$content, [System.Text.UTF8Encoding]::new($$true)) }; doxygen Doxyfile"
else
	@if [ ! -f Doxyfile ]; then echo "Doxyfile not found. Generating template..."; doxygen -g Doxyfile; fi
	doxygen Doxyfile
endif

dist: clean
ifeq ($(OS),Windows_NT)
	@powershell -Command "if (-not (Test-Path dist)) { New-Item -ItemType Directory -Force -Path dist | Out-Null }"
	@powershell -Command "Copy-Item -Path 'Maze.Gui','Maze.Core','Maze.Infrasructure','Maze.Tests','Maze.sln','Makefile' -Destination 'dist' -Recurse -Force"
	@powershell -Command "if (Get-Command tar -ErrorAction SilentlyContinue) { tar -czf $(NAME).tar.gz dist } else { Compress-Archive -Path dist -DestinationPath $(NAME).zip -Force }"
	@powershell -Command "Remove-Item -Recurse -Force dist -ErrorAction SilentlyContinue"
else
	$(MKDIR) dist
	$(CP) Maze.Gui Maze.Core Maze.Infrasructure Maze.Tests Maze.sln Makefile dist/
	tar -czf $(NAME).tar.gz dist
	$(RM) dist
endif

clang:
ifeq ($(OS),Windows_NT)
	@powershell -Command "Copy-Item -Path '../../materials/linters/.clang-format' -Destination '.clang-format' -Force"
	@powershell -Command "$$files = Get-ChildItem -Recurse -Include *.c,*.cpp,*.h,*.hpp | Where-Object { $$_.FullName -notmatch '[\\/]bin[\\/]' -and $$_.FullName -notmatch '[\\/]obj[\\/]' }; $$exitCode = 0; if ($$files) { foreach ($$file in $$files) { clang-format --dry-run -Werror $$file.FullName; if ($$LASTEXITCODE -ne 0) { $$exitCode = 1 } } }; if ($$exitCode -ne 0) { Remove-Item -Force .clang-format -ErrorAction SilentlyContinue; exit 1 }"
	@powershell -Command "if (Test-Path .clang-format) { Remove-Item -Force .clang-format }"
else
	$(COPY) ../../materials/linters/.clang-format .clang-format
	@files=$$(find . -type f \( -name "*.c" -o -name "*.cpp" -o -name "*.h" -o -name "*.hpp" \) -not -path "./bin/*" -not -path "./obj/*" 2>/dev/null); \
	if [ -n "$$files" ]; then \
		echo "$$files" | xargs clang-format --dry-run -Werror || (rm -f .clang-format && exit 1); \
	fi
	rm -f .clang-format
endif

.PHONY: all clean install uninstall tests dvi dist clang