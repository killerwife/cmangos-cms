'use client'

import { useState } from 'react'
import { useCookies } from 'react-cookie';

const changePasswordRequest = (callback: Function, failureCallback: Function, oldPassword: string, newPassword: string, accessToken: string) => {
    fetch('https://localhost:7191/changepassword', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer: ' + accessToken
        },
        body: JSON.stringify({ password: oldPassword, newPassword: newPassword }),
    })
        .then(async (response) => {
            if (response.status == 400) {
                failureCallback((await response.json()) as boolean)
                throw new Error('Old password did not match');
            }
            return response.json()
        })
        .then((r) => {
            callback(r)
        })
}

export default function ChangePassword() {
    const [oldPassword, setOldPassword] = useState('')
    const [newPassword, setNewPassword] = useState('')
    const [newPassword2, setNewPassword2] = useState('')

    const [oldError, setOldError] = useState('')
    const [newError, setNewError] = useState('')
    const [globalError, setGlobalError] = useState('')

    const [cookies] = useCookies(['access-token'])

    const onPasswordChange = () => {
        setOldPassword("");
        setNewPassword("");
        setNewPassword2("");

        setGlobalError("Successfully changed password")
    }

    const onChangeFailure = () => {
        setGlobalError("Old password did not match")
    }

    const onButtonClick = () => {
        setOldError('')
        setNewError('')

        if ('' === oldPassword) {
            setOldError('Please old password')
            return
        }

        if ('' === newPassword || '' === newPassword2) {
            setNewError('Please enter new password')
            return
        }

        if (newPassword != newPassword2) {
            setNewError('Passwords do not match')
            return
        }

        changePasswordRequest(() => { onPasswordChange() }, () => { onChangeFailure() }, oldPassword, newPassword, cookies['access-token'])
    }

    if (cookies['access-token'] == null) {
        return {
            redirect: {
                destination: '/login',
                permanent: false,
            },
        }
    }

    return (
        <div className={'mainContainer'}>
            <div className={'titleContainer'}>
                <div>Login</div>
            </div>
            <br />
            <label className="errorLabel">{globalError}</label>
            <div className={'inputContainer'}>
                <input
                    value={oldPassword}
                    placeholder="Old password"
                    onChange={(ev) => setOldPassword(ev.target.value)}
                    className={'inputBox'}
                />
                <label className="errorLabel">{oldError}</label>
            </div>
            <br />
            <div className={'inputContainer'}>
                <input
                    value={newPassword}
                    placeholder="New password"
                    onChange={(ev) => setNewPassword(ev.target.value)}
                    className={'inputBox'}
                />
                <label className="errorLabel">{newError}</label>
            </div>
            <br />
            <div className={'inputContainer'}>
                <input
                    value={newPassword2}
                    placeholder="New password again"
                    onChange={(ev) => setNewPassword2(ev.target.value)}
                    className={'inputBox'}
                />
            </div>
            <br />
            <div className={'inputContainer'}>
                <input className={'inputButton'} type="button" onClick={onButtonClick} value={'Change password'} />
            </div>
        </div>
    );
}