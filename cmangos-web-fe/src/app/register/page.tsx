'use client'

import { useRef, useState } from 'react'
import { useRouter } from 'next/navigation'
import { env } from 'next-runtime-env';
import { ReCAPTCHA } from 'react-google-recaptcha';
import React from 'react';

const RegisterQuery = () => {
    const [registrationError, setRegistrationError] = useState('')
    const router = useRouter()
    const NEXT_PUBLIC_FOO = env('NEXT_PUBLIC_FOO');

    const onRegisterClick = async (username: string, email: string, password: string, recaptcha: string | null) => {
        fetch(NEXT_PUBLIC_FOO + '/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username: username, password: password, email: email, reCaptchaResponse: recaptcha }),
        })
        .then(async (response) => {
            if (response.status == 400) {
                setRegistrationError('Account with that username already exists')
                throw new Error('Failed to login');
            }
            router.push('/emailVerification')
        })
    }

    return { registrationError, setRegistrationError, onRegisterClick };
}

export default function Register() {
    const recaptcha = useRef<ReCAPTCHA>(null)
    const [username, setUsername] = useState('')
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [password2, setPassword2] = useState('')
    const [usernameError, setUsernameError] = useState('')
    const [passwordError, setPasswordError] = useState('')
    const { registrationError, setRegistrationError, onRegisterClick } = RegisterQuery();
    const siteKey = env('NEXT_PUBLIC_RECAPTCHA_SITE_KEY')

    const onButtonClick = () => {
        if ('' === username) {
            setUsernameError('Please enter your username')
            return
        }

        if ('' === password) {
            setPasswordError('Please enter a password')
            return
        }

        if (password !== password2) {
            setPasswordError('Passwords do not match')
            return
        }

        if ('' === email) {
            setPasswordError('Please enter an email')
            return
        }

        onRegisterClick(username, email, password, recaptcha.current!.getValue())
    }

    return (
        <main>
            <div className={'mainContainer'}>
                <div className={'titleContainer'}>
                    <div>Login</div>
                </div>
                <br />
                <label className="errorLabel">{registrationError}</label>
                <div className={'inputContainer'}>
                    <input
                        value={username}
                        placeholder="Enter username"
                        onChange={(ev) => setUsername(ev.target.value)}
                        className={'inputBox'}
                    />
                    <label className="errorLabel">{usernameError}</label>
                </div>
                <br />
                <div className={'inputContainer'}>
                    <input
                        value={email}
                        placeholder="Enter email"
                        onChange={(ev) => setEmail(ev.target.value)}
                        className={'inputBox'}
                    />
                </div>
                <br />
                <div className={'inputContainer'}>
                    <input
                        value={password}
                        placeholder="Enter password"
                        onChange={(ev) => setPassword(ev.target.value)}
                        className={'inputBox'}
                    />
                </div>
                <br />
                <div className={'inputContainer'}>
                    <input
                        value={password2}
                        placeholder="Enter password again"
                        onChange={(ev) => setPassword2(ev.target.value)}
                        className={'inputBox'}
                    />
                    <label className="errorLabel">{passwordError}</label>
                </div>
                <br />
                <ReCAPTCHA ref={recaptcha} sitekey={siteKey === undefined ? "" : siteKey} />
                <div className={'inputContainer'}>
                    <input className={'inputButton'} type="button" onClick={onButtonClick} value={'Register'} />
                </div>
            </div>
        </main>
    );
}