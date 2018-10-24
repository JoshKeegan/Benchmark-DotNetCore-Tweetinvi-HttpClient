IMAGE = test-twitter-api#

clean:
	rm -rf logs || true
	mkdir logs

build:
	docker build -t $(IMAGE):latest .

run: clean build
	docker run -v ${PWD}/logs:/app/out/logs $(IMAGE):latest