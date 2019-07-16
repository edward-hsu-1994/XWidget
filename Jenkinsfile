pipeline {
    agent any

    stages {
        stage('Build') { 
            steps { 
                echo "Build... $GIT_BRANCH"
                sh "bash ./build.sh";
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
