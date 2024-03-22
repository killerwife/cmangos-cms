'use client'

import { useRouter } from 'next/navigation'
import { useState } from 'react'
import { useCookies } from 'react-cookie';
import { env } from 'next-runtime-env';

export interface plainLoginResult {
    jwtToken: string,
    expiresIn: string, // datetime
    refreshToken: string,
    errors: any // list of strings
}

const authRequest = (callback: Function, failureCallback: Function, username: string, password: string, token: string, cookies: any, setCookie: any) => {
    const parseError = (r: plainLoginResult) => {
        return r.errors.join(', ');
    }

    const NEXT_PUBLIC_API = env('NEXT_PUBLIC_API');

    fetch(NEXT_PUBLIC_API + '/plain/authorize', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ name: username, password: password, pin: token }),
    })
        .then(async (response) => {
            if (response.status == 400) {
                failureCallback(parseError((await response.json()) as plainLoginResult))
                throw new Error('Failed to login');
            }
            return await response.json()
        })
            .then((r: plainLoginResult) => {
                setCookie('access-token', r.jwtToken, { secure: true, sameSite: 'none' })
                setCookie('refresh-token', r.refreshToken, { secure: true, sameSite: 'none' })
                callback(r)
            })
}

export default function Login() {
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [token, setToken] = useState('')
    const [usernameError, setUsernameError] = useState('')
    const [passwordError, setPasswordError] = useState('')
    const [loginError, setLoginError] = useState('')
    const router = useRouter()
    const [cookies, setCookie] = useCookies();

    const loginFailed = (error: string) => {
        setLoginError(error)
    }

    const onButtonClick = () => {
        setUsernameError('')
        setPasswordError('')

        if ('' === username) {
            setUsernameError('Please enter your username')
            return
        }

        if ('' === password) {
            setPasswordError('Please enter a password')
            return
        }

        authRequest(() => { router.push('/') }, loginFailed, username, password, token, cookies, setCookie)
    }

    const onForgotPassword = () => {
        router.push('/forgotpassword')
    }

    return (
        <main>
            <div className={'mainContainer'}>
                <div className={'titleContainer'}>
                    <div>Login</div>
                </div>
                <br />
                <label className="errorLabel">{loginError}</label>
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
                        value={password}
                        placeholder="Enter password"
                        onChange={(ev) => setPassword(ev.target.value)}
                        className={'inputBox'}
                    />
                    <label className="errorLabel">{passwordError}</label>
                </div>
                <br />
                <div className={'inputContainer'}>
                    <input
                        value={token}
                        placeholder="Enter token if applicable"
                        onChange={(ev) => setToken(ev.target.value)}
                        className={'inputBox'}
                    />
                </div>
                <br />
                <div className={'inputContainer'}>
                    <input className={'inputButton'} type="button" onClick={onButtonClick} value={'Log in'} />
                </div>
                <div className={'inputContainer'}>
                    <input className={'inputButton'} type="button" onClick={onForgotPassword} value={'Forgot password'} />
                </div>
            </div>
        </main>
    );
}