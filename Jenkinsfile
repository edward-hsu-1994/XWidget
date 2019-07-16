pipeline {
    agent any

    stages {
        stage('Build') { 
            steps { 
                echo "Build... $GIT_BRANCH"
            }
        }
        stage('Test') {        
            steps {
                echo 'Testing...'
            }
        }
        stage('Deploy') {
            when {
                branch 'master'
            }
            steps {
                echo 'Deploying....'
            }
        }
    }
}
