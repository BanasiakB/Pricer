# Pricer

Prog
Last assignment in QuantScholarship 2023 from Credit Suisse.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)

## Installation

pip install this repo.
(Note: Incompatible with Python 2.x)

```sh
pip3 install sentiment
```

(or)

```sh
pip install sentiment
```

## Usage

### To get help with commandline arguments

```sh
sentiment --help
```

### Using Command-line Arguments

```sh
sentiment -f "some/folder/textfile.txt"
```

(or)

```sh
sentiment -t "Some text"
```

### Print output into terminal

```sh
sentiment -f "some/folder/textfile.txt""
```

### Save output into file

```sh
sentiment -f "some/folder/textfile.txt" -o "some/folder/output_file.txt"
```

(or)

```sh
sentiment -f "some/folder/textfile.txt" > "some/folder/output_file.txt"
```

### Disclaimer

sometimes the sentiment command doesn't work in windows if the package is installed globally.

to avoid this, install the package in a local virtual env

first, create a env

```sh
python3 -m venv env_for_sentiment
```

activate that env

```sh
.\env_for_sentiment\Scripts\activate
```

and then pip install. But you will have to activate that env everytime you want to use sentiment.

## Development setup

Clone this repo and install packages listed in requirements.txt

```sh
pip3 install -r requirements.txt
```

## Meta

Distributed under the MIT license. See `LICENSE` for more information.

[https://github.com/BanasiakB/](https://github.com/BanasiakB/)

## Contributing

1. Fork it (<https://github.com/BanasiakB/Sentiment/fork>)
2. Create your feature branch (`git checkout -b feature/fooBar`)
3. Commit your changes (`git commit -am 'Add some fooBar'`)
4. Push to the branch (`git push origin feature/fooBar`)
5. Create a new Pull Request
