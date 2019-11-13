flags = -lglut -lGLU -lGL -Wall

all: build

run: build
	./run

build: main.o

main.o: main.cpp
	@g++ main.cpp -o run $(flags)

clean:
	@rm run* *.o
