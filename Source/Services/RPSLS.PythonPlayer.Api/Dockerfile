FROM python:3.7
WORKDIR /app
COPY requirements.txt .
RUN pip install -r requirements.txt
COPY . .
CMD ["gunicorn", "--workers", "1", "--threads", "5", "--bind", ":5000", "app:app"]